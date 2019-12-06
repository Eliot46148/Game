using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Chunk                              // 區塊
{

    World world;                                // 方塊世界本體
    public ChunkModel model;

    GameObject chunkObject;                     // 區塊本體

    MeshFilter meshFilter;                      // 區塊網格
    MeshRenderer meshRenderer;                  // 區塊材質

    private bool _isActive;                     // 區塊顯示

    // 材質變數
    Material[] materials = new Material[2];

    // Constructor
    public Chunk(ChunkCoord _coord, World _world)
    {
        model = new ChunkModel(_coord, _world);
        world = _world;
    }

    // 初始化
    public void Init(List<VoxelMod> data = null)
    {
        // 創建區塊
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        model.position = new Vector3s(model.coord.x * VoxelData.ChunkWidth, 0f, model.coord.z * VoxelData.ChunkWidth);
        chunkObject.transform.position = World.sv(model.position);
        
        // 定義材質
        materials[0] = world.material;
        materials[1] = world.transparentMaterial;
        meshRenderer.materials = materials;

        // Scene整理
        chunkObject.transform.SetParent(world.transform);
        chunkObject.name = "Chunk " + model.coord.x + ", " + model.coord.z;

        model.PopulateVoxelMap(data);

        // 多執行緒
        lock (world.ChunkUpdateThreadLock)
            world.chunksToUpdate.Add(this);

        // 生成升降動畫
        if (world.settings.enableAnimatedChunks)
            chunkObject.AddComponent<ChunkLoadAnimation>();
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
            if (!model.isVoxelMapPopulated)
                return false;
            else
                return true;
        }

    }

    // 優先修改一個方塊(與周圍) 由玩家控制等
    public void EditVoxel(Vector3 pos, BlockType newID)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        model.modificationsRecord.Add(new VoxelMod(new Vector3s(xCheck, yCheck, zCheck), newID));
        model.voxelMap[xCheck, yCheck, zCheck].id = newID;

        lock (world.ChunkUpdateThreadLock)
        {
            world.chunksToUpdate.Insert(0, this);
            model.UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
        }
    }

    public BlockType GetBlockID(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        return model.voxelMap[xCheck, yCheck, zCheck].id;
    }

    // 繪製區塊
    public void CreateMesh()
    {
        Mesh mesh = new Mesh();

        // 繪製網格
        mesh.vertices = World.sl3vl(model.vertices).ToArray();
        mesh.subMeshCount = 2;
        mesh.SetTriangles(model.triangles.ToArray(), 0);
        mesh.SetTriangles(model.transparentTriangles.ToArray(), 1);
        mesh.normals = World.sl3vl(model.normals).ToArray();

        // 繪製材質
        mesh.uv = World.sl2vl(model.uvs).ToArray();
        mesh.colors = World.sl2cl(model.colors).ToArray();

        meshFilter.mesh = mesh;

    }

    internal void UpdateChunk()
    {
        model.UpdateChunk();
        world.chunksToDraw.Enqueue(this);
    }
}

// 方塊座標
[Serializable]
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

    public ChunkCoord(Vector3s pos)
    {
        x = (int)(pos.x / VoxelData.ChunkWidth);
        z = (int)(pos.z / VoxelData.ChunkWidth);
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
[Serializable]
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
[Serializable]
public class VoxelMod
{
    public Vector3s position;    // 修改方塊位置
    public BlockType id;        // 修改方塊ID

    public VoxelMod()
    {
        position = new Vector3s();
        id = 0;
    }

    public VoxelMod(Vector3s _position, BlockType _id)
    {
        position = _position;
        id = _id;
    }
}