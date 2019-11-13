using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk                              // 區塊
{

    World world;                                // 方塊世界本體

    GameObject chunkObject;                     // 區塊本體
    public Vector3 position;                    // 區塊實際座標
    public ChunkCoord coord;                    // 區塊方塊座標

    MeshFilter meshFilter;                      // 區塊網格
    MeshRenderer meshRenderer;                  // 區塊材質

    private bool _isActive;                     // 區塊顯示
    private bool isVoxelMapPopulated = false;   // 區塊生成布林

    // 方塊矩陣
    public VoxelState[,,] voxelMap = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    // 網格變數
    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<int> transparentTriangles = new List<int>();
    List<Vector3> normals = new List<Vector3>();

    // 材質變數
    Material[] materials = new Material[2];
    List<Vector2> uvs = new List<Vector2>();

    // 材質光影
    List<Color> colors = new List<Color>();

    // 預定修改方塊矩陣
    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    // Constructor
    public Chunk(ChunkCoord _coord, World _world)
    {
        coord = _coord;
        world = _world;
    }

    // 初始化
    public void Init()
    {
        // 創建區塊
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        chunkObject.transform.position = position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        
        // 定義材質
        materials[0] = world.material;
        materials[1] = world.transparentMaterial;
        meshRenderer.materials = materials;

        // Scene整理
        chunkObject.transform.SetParent(world.transform);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

        PopulateVoxelMap();
    }

    // 定義方塊矩陣
    void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                    voxelMap[x, y, z] = new VoxelState(world.GetInitialVoxel(new Vector3(x, y, z) + position));

        isVoxelMapPopulated = true;

        // 多執行緒
        lock (world.ChunkUpdateThreadLock)
            world.chunksToUpdate.Add(this);

        // 生成升降動畫
        if (world.settings.enableAnimatedChunks)
            chunkObject.AddComponent<ChunkLoadAnimation>();
    }

    // 修改方塊矩陣
    public void UpdateChunk()
    {
        while (modifications.Count > 0)
        {
            VoxelMod v = modifications.Dequeue();
            Vector3 pos = v.position -= position;
            voxelMap[(int)pos.x, (int)pos.y, (int)pos.z].id = v.id;
        }

        ClearMeshData();
        CalculateLight();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                    if (world.blocks[(int)voxelMap[x, y, z].id].isSolid)
                        UpdateMeshData(new Vector3(x, y, z), Color.green);

        world.chunksToDraw.Enqueue(this);
    }

    // 計算亮度
    void CalculateLight()
    {
        Queue<Vector3Int> litVoxels = new Queue<Vector3Int>();

        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int z = 0; z < VoxelData.ChunkWidth; z++)
            {
                float lightRay = 1f;

                for (int y = VoxelData.ChunkHeight - 1; y >= 0; y--)
                {

                    VoxelState thisVoxel = voxelMap[x, y, z];

                    if (thisVoxel.id > 0 && world.blocks[(int)thisVoxel.id].transparency < lightRay)
                        lightRay = world.blocks[(int)thisVoxel.id].transparency;

                    thisVoxel.globalLightPercent = lightRay;

                    voxelMap[x, y, z] = thisVoxel;

                    if (lightRay > VoxelData.lightFalloff)
                        litVoxels.Enqueue(new Vector3Int(x, y, z));
                }
            }
        }

        while (litVoxels.Count > 0)
        {
            Vector3Int v = litVoxels.Dequeue();

            for (int p = 0; p < 6; p++)
            {
                Vector3 currentVoxel = v + VoxelData.faceChecks[p];
                Vector3Int neighbor = new Vector3Int((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z);

                if (IsVoxelInChunk(neighbor.x, neighbor.y, neighbor.z))
                {
                    if (voxelMap[neighbor.x, neighbor.y, neighbor.z].globalLightPercent < voxelMap[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFalloff)
                    {
                        voxelMap[neighbor.x, neighbor.y, neighbor.z].globalLightPercent = voxelMap[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFalloff;
                        if (voxelMap[neighbor.x, neighbor.y, neighbor.z].globalLightPercent > VoxelData.lightFalloff)
                            litVoxels.Enqueue(neighbor);
                    }
                }
            }
        }
    }

    // 清除網格及材質變數
    void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        transparentTriangles.Clear();
        uvs.Clear();
        colors.Clear();
        normals.Clear();
    }

    // 區塊顯示 Property
    public bool isActive
    {

        get { return _isActive; }
        set
        {
            _isActive = value;
            if (chunkObject != null)
                chunkObject.SetActive(value);
        }

    }

    // 可否修改此區塊 Property
    public bool isEditable
    {

        get
        {
            if (!isVoxelMapPopulated)
                return false;
            else
                return true;
        }

    }

    // 方塊是否在區塊中
    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        else
            return true;
    }

    // 優先修改一個方塊(與周圍) 由玩家控制等
    public void EditVoxel(Vector3 pos, BlockType newID)
    {

        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        voxelMap[xCheck, yCheck, zCheck].id = newID;

        lock (world.ChunkUpdateThreadLock)
        {
            world.chunksToUpdate.Insert(0, this);
            UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
        }

    }

    // 修改周圍方塊
    void UpdateSurroundingVoxels(int x, int y, int z)
    {
        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++)
        {
            Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
                world.chunksToUpdate.Insert(0, world.GetChunkFromVector3(currentVoxel + position));
        }
    }

    // 用方格座標取得方塊
    VoxelState CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.GetVoxelState(pos + position);

        return voxelMap[x, y, z];
    }

    // 用絕對座標取得方塊
    public VoxelState GetVoxelFromGlobalVector3(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(position.x);
        zCheck -= Mathf.FloorToInt(position.z);

        return voxelMap[xCheck, yCheck, zCheck];
    }

    // 刷新網格與材質變數
    void UpdateMeshData(Vector3 pos, Color color)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        BlockType blockID = voxelMap[x, y, z].id;

        // 刷新六面資料
        for (int p = 0; p < 6; p++)
        {
            VoxelState neighbor = CheckVoxel(pos + VoxelData.faceChecks[p]);
            // 周邊方塊不會擋住此方塊才顯示
            if (neighbor != null && world.blocks[(int)neighbor.id].renderNeighborFaces)
            {
                float lightLevel = neighbor.globalLightPercent;

                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);            // 網格座標
                    normals.Add(VoxelData.faceChecks[p]);                                           // 網格方向
                    colors.Add(new Color(color.r, color.g, color.b, lightLevel)); // 材質亮度
                }

                AddTexture(world.blocks[(int)blockID].GetTextureID(p));

                if (!world.blocks[(int)neighbor.id].renderNeighborFaces)    // 不透明材質 的兩個三角形座標
                {
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 3);
                }
                else                                                        // 透明材質 的兩個三角形座標
                {
                    transparentTriangles.Add(vertexIndex);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 3);
                }
                vertexIndex += 4;
            }
        }
    }

    // 繪製區塊
    public void CreateMesh()
    {
        Mesh mesh = new Mesh();

        // 繪製網格
        mesh.vertices = vertices.ToArray();
        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(transparentTriangles.ToArray(), 1);
        mesh.normals = normals.ToArray();

        // 繪製材質
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();

        meshFilter.mesh = mesh;

    }

    // 增加材質座標
    void AddTexture(Vector2 side)
    {
        float x = side.y / world.basic.tileX;
        float y = side.x / world.basic.tileY;
        float textureSizeX = 1f / world.basic.tileX;
        float textureSizeY = 1f / world.basic.tileY;
        y = 1f - y - textureSizeY;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + textureSizeY));
        uvs.Add(new Vector2(x + textureSizeX, y));
        uvs.Add(new Vector2(x + textureSizeX, y + textureSizeY));
    }

}

// 方塊座標
public class ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord()
    {
        x = 0;
        z = 0;
    }

    public ChunkCoord(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public ChunkCoord(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int zCheck = Mathf.FloorToInt(pos.z);

        x = xCheck / VoxelData.ChunkWidth;
        z = zCheck / VoxelData.ChunkWidth;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;
    }
}

// 方塊Data
public class VoxelState
{
    public BlockType id;                // 方塊ID
    public float globalLightPercent;    // 方塊亮度

    public VoxelState()
    {
        id = 0;
        globalLightPercent = 0f;
    }

    public VoxelState(BlockType _id)
    {
        id = _id;
        globalLightPercent = 0f;
    }
}

// 修改方塊Data
public class VoxelMod
{
    public Vector3 position;    // 修改方塊位置
    public BlockType id;        // 修改方塊ID

    public VoxelMod()
    {
        position = new Vector3();
        id = 0;
    }

    public VoxelMod(Vector3 _position, BlockType _id)
    {
        position = _position;
        id = _id;
    }
}