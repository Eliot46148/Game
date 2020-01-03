using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class World : MonoBehaviour
{
    [Header("主遊戲設定")]
    public Settings settings;
    public SaveData data = null;

    [Header("世界生成參數")]
    public BiomeAttributes[] biomes;
    public Color mid;
    public NavMeshSurface navMeshSurface;
    // 全域光源設定
    [Range(0f, 1f)]
    public float globalLightLevel;
    public Color day;
    public Color night;

    // 方塊設定
    public Basic basic;
    public Material material;
    public Material transparentMaterial;

    [Header("玩家設定")]
    public Transform player;
    Vector3 _spawnPosition;
    public bool _isPlayerPlace = false;
    ChunkCoord _playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    // 區塊矩陣
    public Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();     // 顯示中的區塊座標矩陣
    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();   // 預計被生成的區塊座標陣列
    List<Chunk> _chunksToUpdate = new List<Chunk>();            // 預計被更新的區塊陣列
    Queue<Chunk> _chunksToDraw = new Queue<Chunk>();            // 預計被繪製的區塊陣列

    // 生態區更新變數
    Queue<Queue<VoxelMod>> modifications = new Queue<Queue<VoxelMod>>();
    bool applyingModifications = false;

    int _inUI = 0;  // UI模式 (0為不能動)
    public GameObject loading;

    // 多執行緒變數
    Thread ChunkUpdateThread;
    public object ChunkUpdateThreadLock = new object();
    List<WrappingClass> saveModifications = new List<WrappingClass>();

    public ItemController itemController;    

    private void Start()
    {
        loading.SetActive(true); // 讀取中

        try
        {
            // 讀取設定檔
            string jsonImport = File.ReadAllText(Application.dataPath + "/settings.cfg");
            settings = JsonUtility.FromJson<Settings>(jsonImport);
        }
        catch
        {
            // 儲存設定檔
            string jsonExport = JsonUtility.ToJson(settings);
            Debug.Log("Generating settings file - " + jsonExport);
            File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);
        }

        try
        {
            //throw new System.InvalidOperationException("Debugging");
            string jsonImport = File.ReadAllText(Application.dataPath + "/saveData.save");
            data = JsonUtility.FromJson<SaveData>(jsonImport);
            spawnPosition = data.PlayerPosition;
            player.rotation = data.PlayerRotation;
            foreach (string json in data.VoxelMaps)
                saveModifications.Add(JsonUtility.FromJson<WrappingClass>(json));
        }
        catch
        {
            spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight - 50f, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);
        }

        // 初始化亂數
        Random.InitState(settings.seed);
        // 初始化光源設定
        Shader.SetGlobalFloat("minGlobalLightLevel", VoxelData.minLightLevel);
        Shader.SetGlobalFloat("maxGlobalLightLevel", VoxelData.maxLightLevel);
        Shader.SetGlobalFloat("GlobalLightLevel", globalLightLevel);
        Camera.main.backgroundColor = Color.Lerp(night, day, globalLightLevel);
        // 生成世界
        if (settings.enableThreading)
        {
            ChunkUpdateThread = new Thread(new ThreadStart(ThreadedUpdate));
            ChunkUpdateThread.Start();
        }
        GenerateWorld(5);
        playerLastChunkCoord = GetChunkCoordFromVector3s(vs(player.position));        
    }

    private void Update()
    {
        Camera.main.backgroundColor = Color.Lerp(night, day, globalLightLevel);
        // 更新玩家所在區塊座標
        playerChunkCoord = GetChunkCoordFromVector3s(vs(player.position));

        // 如果玩家所在區塊座標改變，就更新視野範圍
        if (!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();

        // 生成預計被生成的區塊
        if (chunksToCreate.Count > 0)
            CreateChunk();

        // 繪製預計被繪製的區塊
        if (chunksToDraw.Count > 0)
            if (chunksToDraw.Peek().isEditable)
                chunksToDraw.Dequeue().CreateMesh();

        // 更新預計被更新的區塊
        if (!settings.enableThreading)
        {
            if (!applyingModifications)
                ApplyModifications();

            if (chunksToUpdate.Count > 0) { }
            UpdateChunks();
        }

        //navMeshSurface.BuildNavMesh();
    }

    // 生成主要副程式
    void GenerateWorld(int preGenerateRadius = -1)
    {
        preGenerateRadius = (preGenerateRadius == -1 ? settings.viewDistance : preGenerateRadius);

        /*
        if (data != null)
        {
            foreach (string json in data.VoxelMaps)
            {
                WrappingClass wrapper = JsonUtility.FromJson<WrappingClass>(json);
                chunks[wrapper.Coordinate.x, wrapper.Coordinate.z] = new Chunk(wrapper.Coordinate, this);
                chunks[wrapper.Coordinate.x, wrapper.Coordinate.z].Init(wrapper.Modifications);
                _isPlayerPlace = true;
                loading.SetActive(false); // 讀取結束
                UIState = 1;
            }
        }
        else
        {*/
        ChunkCoord playerChunk = GetChunkCoordFromVector3s(vs(spawnPosition));
        for (int x =-preGenerateRadius; x < preGenerateRadius; x++)
            for (int z = -preGenerateRadius; z < preGenerateRadius; z++)
                if (Mathf.Pow((Mathf.Pow(x, 2f) + Mathf.Pow(z, 2f)), 0.5f) < preGenerateRadius)
                {
                    ChunkCoord newChunk = new ChunkCoord(playerChunk.x + x, playerChunk.z + z);
                    chunks[newChunk.x, newChunk.z] = new Chunk(newChunk, this);
                    chunksToCreate.Add(newChunk);
                }
        /*
        }*/

        // 生成玩家位置
        _isPlayerPlace = true;
        loading.SetActive(false); // 讀取結束
        UIState = 1;
        player.position = spawnPosition;
        CheckViewDistance();
    }

    // 生成預計被生成的區塊陣列
    void CreateChunk()
    {
        ChunkCoord c = chunksToCreate[0];
        chunksToCreate.RemoveAt(0);

        WrappingClass modification = null;
        foreach (WrappingClass mod in saveModifications)
            if (mod.Coordinate == c)
                modification = mod;

        if (modification != null)
        {
            chunks[c.x, c.z].Init(modification.Modifications);
            saveModifications.Remove(modification);
        }
        else
            chunks[c.x, c.z].Init();

        if (!_isPlayerPlace && c.Equals(playerChunkCoord))
        {
            spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight - 128f, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);
            _isPlayerPlace = true;
            loading.SetActive(false); // 讀取結束
            UIState = 1;
        }
    }

    // 更新 預計被更新的區塊陣列
    void UpdateChunks()
    {
        bool updated = false;
        int index = 0;

        lock (ChunkUpdateThreadLock)
            while (!updated && index < chunksToUpdate.Count - 1)
            {
                if (chunksToUpdate[index].isEditable)
                {
                    chunksToUpdate[index].UpdateChunk();
                    if (!activeChunks.Contains(chunksToUpdate[index].model.coord))
                        activeChunks.Add(chunksToUpdate[index].model.coord);
                    chunksToUpdate.RemoveAt(index);
                    updated = true;
                }
                else
                    index++;
            }
    }

    // 多執行緒計算更新資料
    void ThreadedUpdate()
    {
        while (settings.enableThreading)
        {
            if (!applyingModifications)
                ApplyModifications();

            if (chunksToUpdate.Count > 0)
                UpdateChunks();
        }
    }

    // 當結束遊戲時跳出多執行緒
    private void OnDisable()
    {
        if (settings.enableThreading)
        {
            settings.enableThreading = false;
            ChunkUpdateThread.Abort();
        }
        UIState = 0;
    }

    // 處理生態區的改變
    void ApplyModifications()
    {
        // 鎖住改變
        applyingModifications = true;

        while (modifications.Count > 0)
        {
            Queue<VoxelMod> queue = modifications.Dequeue();
            while (queue.Count > 0)
            {
                VoxelMod v = queue.Dequeue();
                ChunkCoord c = GetChunkCoordFromVector3s(v.position);

                if (chunks[c.x, c.z] == null)
                {
                    chunks[c.x, c.z] = new Chunk(c, this);
                    chunksToCreate.Add(c);
                }
                chunks[c.x, c.z].model.modifications.Enqueue(v);
            }
        }
        // 釋放改變
        applyingModifications = false;
    }

    // 從實際座標得到區塊座標
    ChunkCoord GetChunkCoordFromVector3s(Vector3s pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }

    // 從實際座標得到區塊
    public Chunk GetChunkFromVector3s(Vector3s pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return chunks[x, z];
    }

    // 更新玩家視野
    void CheckViewDistance()
    {
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
        ChunkCoord coord = GetChunkCoordFromVector3s(vs(player.position));
        playerLastChunkCoord = playerChunkCoord;
        activeChunks.Clear();

        // 迴圈所有視野範圍內的區塊 ( 以玩家為中心 直徑為(視野)的圓形 的矩陣大小 )
        for (int x = coord.x - settings.viewDistance; x < coord.x + settings.viewDistance; x++)
        {
            for (int z = coord.z - settings.viewDistance; z < coord.z + settings.viewDistance; z++)
            {
                ChunkCoord newChunk = new ChunkCoord(x - coord.x, z - coord.z);
                if (Mathf.Pow((Mathf.Pow(newChunk.x, 2f) + Mathf.Pow(newChunk.z, 2f)), 0.5f) < settings.viewDistance)
                {
                    ChunkCoord thisChunkCoord = new ChunkCoord(x, z);

                    // 如果區塊存在於世界中
                    if (IsChunkInWorld(thisChunkCoord))
                    {
                        // 如果區塊還沒被生成
                        if (chunks[x, z] == null)
                        {
                            chunks[x, z] = new Chunk(thisChunkCoord, this);
                            chunksToCreate.Add(thisChunkCoord);
                        }
                        else if (!chunks[x, z].isActive)
                        {
                            chunks[x, z].isActive = true;
                        }
                        activeChunks.Add(thisChunkCoord);
                    }

                    // 如果區塊已經顯示那麼不再重複顯示
                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                        if (previouslyActiveChunks[i].Equals(thisChunkCoord))
                            previouslyActiveChunks.RemoveAt(i);
                }
            }
        }

        // 剩下的區塊屬於玩家視野外所以關閉顯示
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
    }

    // 用實際座標確認方塊是否為實體
    public bool CheckForVoxel(Vector3s pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isEditable)

            return blocks[(int)chunks[thisChunk.x, thisChunk.z].model.GetVoxelFromGlobalVector3s(pos).id].isSolid;

        return blocks[(int)GetInitialVoxel(pos)].isSolid;
    }

    // 用實際座標得到方塊
    public VoxelState GetVoxelState(Vector3s pos)
    {

        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return null;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isEditable)
            return chunks[thisChunk.x, thisChunk.z].model.GetVoxelFromGlobalVector3s(pos);

        return new VoxelState(GetInitialVoxel(pos));

    }

    // 更改UI模式的 Property
    public int UIState
    {
        get { return _inUI; }
        set
        {
            _inUI = value;
            switch (_inUI)
            {
                case 0:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case 1:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
                default:
                    break;
            }
        }
    }

    public List<Chunk> chunksToUpdate
    {
        get
        {
            return _chunksToUpdate;
        }

        set
        {
            _chunksToUpdate = value;
        }
    }

    public Queue<Chunk> chunksToDraw
    {
        get
        {
            return _chunksToDraw;
        }

        set
        {
            _chunksToDraw = value;
        }
    }

    // 生成座標並轉換成地圖平面
    public Vector3 spawnPosition
    {
        get { return _spawnPosition; }
        set
        {
            _spawnPosition = value;
            // 玩家所在區塊
            Chunk chunk = GetChunkFromVector3s(vs(value));
            if (chunk != null)
            {
                Vector3s chunkVoxelPos = vs(value) - chunk.model.position;
                ChunkCoord coord = new ChunkCoord(chunkVoxelPos);
                int i;
                for (i = VoxelData.ChunkHeight - 1; i > -1; i--)
                    if (chunk.model.voxelMap[coord.x, i, coord.z].id != BlockType.Air)
                        break;
                player.position = _spawnPosition = new Vector3(_spawnPosition.x, i + 1.5f, _spawnPosition.z);
            }
        }
    }

    public ChunkCoord playerChunkCoord
    {
        get
        {
            return _playerChunkCoord;
        }

        set
        {
            _playerChunkCoord = value;
        }
    }

    public List<BasicBlock> blocks
    {
        get
        {
            return basic.Blocks;
        }

        set
        {
            basic.Blocks = value;
        }
    }

    // 用實際座標取得生態圈的方塊
    public BlockType GetInitialVoxel(Vector3s pos)
    {
        int yPos = Mathf.FloorToInt(pos.y);

        /* 不變規則 */

        // 超出世界回傳空氣
        if (!IsVoxelInWorld(pos))
            return BlockType.Air;

        // 達到世界底部回傳基岩
        if (yPos == 0)
            return BlockType.Bedrock;

        /* 生態圈選擇規則 */
        int solidGroundHeight = 42;
        float sumOfHeights = 0f;
        int count = 0;
        float strongestWeight = 0f;
        int strongestBiomeIndex = 0;

        for (int i = 0; i < biomes.Length; i++)
        {
            float weight = Noise.Get2DPerlin(new Vector2(pos.x, pos.z), biomes[i].offset, biomes[i].scale);
            // 檢查哪個生態圈的加權值 (Perlin雜訊值) 較高
            if (weight > strongestWeight)
            {
                strongestWeight = weight;
                strongestBiomeIndex = i;
            }

            // 計算生態圈的高度 (雜訊值 * 設定高度)
            float height = biomes[i].terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biomes[i].terrainScale) * weight;

            // 如果高度有效那麼加進平均
            if (height > 0)
            {
                sumOfHeights += height;
                count++;
            }
        }

        // 設定最高加權值的生態圈
        BiomeAttributes biome = biomes[strongestBiomeIndex];

        // 生態圈平均高度
        sumOfHeights /= count;
        int terrainHeight = Mathf.FloorToInt(sumOfHeights + solidGroundHeight);

        /* 生態圈地表規則 */
        BlockType voxelValue = 0;

        if (yPos == terrainHeight)
            voxelValue = biome.surfaceBlock;
        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            voxelValue = biome.subSurfaceBlock;
        else if (yPos > terrainHeight)
            return BlockType.Air;
        else
            voxelValue = BlockType.Stone;

        /* 生態圈內部礦物規則 */
        if (voxelValue == BlockType.Stone)
            foreach (Lode lode in biome.lodes)
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                    if (Noise.Get3DPerlin(sv(pos), lode.noiseOffset, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;

        /* 生態圈表面植被規則 */
        if (yPos == terrainHeight && biome.placeMajorFlora)
            if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.majorFloraZoneScale) > biome.majorFloraZoneThreshold)
                if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.majorFloraPlacementScale) > biome.majorFloraPlacementThreshold)
                    modifications.Enqueue(Structure.GenerateMajorFlora(biome.majorFloraIndex, sv(pos), biome.minHeight, biome.maxHeight));

        return voxelValue;


    }

    // 用區塊座標確認區塊是否在世界中
    bool IsChunkInWorld(ChunkCoord coord)
    {

        if (coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelData.WorldSizeInChunks - 1)
            return true;
        else
            return
                false;

    }

    // 用實際座標確認方塊是否在世界中
    bool IsVoxelInWorld(Vector3s pos)
    {

        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;

    }

    public static Vector3s vs(Vector3 a)
    {
        return new Vector3s(a.x, a.y, a.z);
    }

    public static Vector3 sv(Vector3s a)
    {
        return new Vector3(a.x, a.y, a.z);
    }

    public static Vector2 s2v(Vector2s a)
    {
        return new Vector2(a.x, a.y);
    }

    public static Color sc(Colors a)
    {
        return new Color(a.r, a.g, a.b, a.a);
    }

    public static Color[] sl2cl(List<Colors> a)
    {
        int n = a.Count;
        Color[] output = new Color[n];
        for (int i = 0; i < n; i++)
            output[i] = sc(a[i]);
        return output;
    }

    public static Vector3[] sl3vl(List<Vector3s> a)
    {
        int n = a.Count;
        Vector3[] output = new Vector3[n];
        for (int i = 0; i < n; i++)
            output[i] = sv(a[i]);
        return output;
    }

    public static Vector2[] sl2vl(List<Vector2s> a)
    {
        int n = a.Count;
        Vector2[] output = new Vector2[n];
        for (int i = 0; i < n; i++)
            output[i] = s2v(a[i]);
        return output;
    }
    public void ExitAndSave()
    {
        List<string> map = new List<string>();
        List<string> bagST = new List<string>();
        List<string> toolbarST = new List<string>();
        for (int mx = 0; mx < VoxelData.WorldSizeInChunks; mx++)
            for (int mz = 0; mz < VoxelData.WorldSizeInChunks; mz++)
                if (chunks[mx, mz] != null && chunks[mx, mz].model.modificationsRecord.Count > 0)
                    map.Add(JsonUtility.ToJson(new WrappingClass(chunks[mx, mz].model.coord, chunks[mx, mz].model.modificationsRecord), true));

        foreach (var item in itemController.bag)
            if (item != null)
                bagST.Add(JsonUtility.ToJson(new SaveItem(item.itemSlot.stack.id, item.itemSlot.stack.amount), true));
        foreach (var item in itemController.toolbar)
            if (item != null && item.HasItem)
                toolbarST.Add(JsonUtility.ToJson((item.itemSlot.stack.id, item.itemSlot.stack.amount)));
            else if (item != null && !item.HasItem)
                toolbarST.Add(JsonUtility.ToJson((0, 0)));

        File.WriteAllText(Application.dataPath + "/saveData.save", JsonUtility.ToJson(new SaveData(player.position, player.rotation, bagST, toolbarST, map), true));
        Exit();
    }

    public void Exit()
    {
        UIState = 1;
        SceneManager.LoadScene(0);
    }
}

[System.Serializable]
public class Settings
{

    [Header("遊戲資訊")]
    public string version;

    [Header("效能")]
    public int viewDistance;
    public bool enableThreading;
    public bool enableAnimatedChunks;

    [Header("控制")]
    [Range(0.1f, 10f)]
    public float mouseSensitivity;

    [Header("種子")]
    public int seed;

}