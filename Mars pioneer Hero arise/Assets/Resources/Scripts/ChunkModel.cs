using System.Collections;
using System.Collections.Generic;

public class ChunkModel
{
    World world;                                // 方塊世界本體

    public Vector3s position;                    // 區塊實際座標
    public ChunkCoord coord;                    // 區塊方塊座標

    public bool isVoxelMapPopulated = false;   // 區塊生成布林

    // 方塊矩陣
    public VoxelState[,,] voxelMap = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    // 網格變數
    int vertexIndex = 0;
    public List<Vector3s> vertices = new List<Vector3s>();
    public List<int> triangles = new List<int>();
    public List<int> transparentTriangles = new List<int>();
    public List<Vector3s> normals = new List<Vector3s>();

    // 材質變數
    public List<Vector2s> uvs = new List<Vector2s>();

    // 材質光影
    public List<Colors> colors = new List<Colors>();

    // 預定修改方塊矩陣
    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    // Constructor
    public ChunkModel(ChunkCoord _coord, World _world)
    {
        coord = _coord;
        world = _world;
    }

    // 定義方塊矩陣
    public void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                    voxelMap[x, y, z] = new VoxelState(world.GetInitialVoxel(new Vector3s(x, y, z) + position));

        isVoxelMapPopulated = true;
    }

    // 修改方塊矩陣
    public void UpdateChunk()
    {
        while (modifications.Count > 0)
        {
            VoxelMod v = modifications.Dequeue();
            Vector3s pos = v.position -= position;
            voxelMap[(int)pos.x, (int)pos.y, (int)pos.z].id = v.id;
        }

        ClearMeshData();
        //CalculateLight();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                    if (world.blocks[(int)voxelMap[x, y, z].id].isSolid)
                        UpdateMeshData(new Vector3s(x, y, z));
    }

    // 計算亮度
    public void CalculateLight()
    {
        Queue<Vector3s> litVoxels = new Queue<Vector3s>();

        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int z = 0; z < VoxelData.ChunkWidth; z++)
            {
                float lightRay = 1f;

                for (int y = VoxelData.ChunkHeight - 1; y > -1; y--)
                {
                    VoxelState thisVoxel = voxelMap[x, y, z];

                    if (thisVoxel.id > 0 && world.blocks[(int)thisVoxel.id].transparency < lightRay)
                        lightRay = world.blocks[(int)thisVoxel.id].transparency;

                    thisVoxel.globalLightPercent = lightRay;

                    voxelMap[x, y, z] = thisVoxel;

                    if (lightRay > VoxelData.lightFalloff)
                        litVoxels.Enqueue(new Vector3s(x, y, z));
                }
            }
        }
        
        while (litVoxels.Count > 0)
        {
            Vector3s v = litVoxels.Dequeue();

            for (int p = 0; p < 6; p++)
            {
                Vector3s currentVoxel = v + VoxelData.faceChecks[p];
                int xCheck = (int)currentVoxel.x;
                int yCheck = (int)currentVoxel.y;
                int zCheck = (int)currentVoxel.z;
                Vector3s neighbor = new Vector3s(currentVoxel.x, currentVoxel.y, currentVoxel.z);

                if (IsVoxelInChunk(xCheck, yCheck, zCheck))
                {
                    if (voxelMap[xCheck, yCheck, zCheck].globalLightPercent < voxelMap[(int)v.x, (int)v.y, (int)v.z].globalLightPercent - VoxelData.lightFalloff)
                    {
                        voxelMap[xCheck, yCheck, zCheck].globalLightPercent = voxelMap[(int)v.x, (int)v.y, (int)v.z].globalLightPercent - VoxelData.lightFalloff;
                        if (voxelMap[xCheck, yCheck, zCheck].globalLightPercent > VoxelData.lightFalloff)
                            litVoxels.Enqueue(neighbor);
                    }
                }
            }
        }
    }

    // 清除網格及材質變數
    public void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        transparentTriangles.Clear();
        uvs.Clear();
        colors.Clear();
        normals.Clear();
    }

    // 方塊是否在區塊中
    public bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        else
            return true;
    }

    // 修改周圍方塊
    public void UpdateSurroundingVoxels(int x, int y, int z)
    {
        Vector3s thisVoxel = new Vector3s(x, y, z);

        for (int p = 0; p < 6; p++)
        {
            Vector3s currentVoxel = thisVoxel + VoxelData.faceChecks[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
                world.chunksToUpdate.Insert(0, world.GetChunkFromVector3s(currentVoxel + position));
        }
    }

    // 用方格座標取得方塊
    public VoxelState CheckVoxel(Vector3s pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        if (!IsVoxelInChunk(x, y, z))
            return world.GetVoxelState(pos + position);

        return voxelMap[x, y, z];
    }

    // 用絕對座標取得方塊
    public VoxelState GetVoxelFromGlobalVector3s(Vector3s pos)
    {
        int xCheck = (int)pos.x;
        int yCheck = (int)pos.y;
        int zCheck = (int)pos.z;

        xCheck -= (int)position.x;
        zCheck -= (int)position.z;

        return voxelMap[xCheck, yCheck, zCheck];
    }

    // 刷新網格與材質變數
    public void UpdateMeshData(Vector3s pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

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
                    //colors.Add(new Colors(0, 0, 0, lightLevel));                                    // 材質亮度
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

    // 增加材質座標
    public void AddTexture(Vector2s side)
    {
        float textureSizeCols = 1f / world.basic.tileCols;
        float textureSizeRows = 1f / world.basic.tileRows;
        float c1 = side.y / world.basic.tileCols;
        float r1 = side.x / world.basic.tileRows;
        r1 = 1f - r1 - textureSizeRows;
        float c2 = c1 + textureSizeCols;
        float r2 = r1 + textureSizeRows;

        uvs.Add(new Vector2s(c1, r1));
        uvs.Add(new Vector2s(c1, r2));
        uvs.Add(new Vector2s(c2, r1));
        uvs.Add(new Vector2s(c2, r2));
    }

}

[System.Serializable]
public class Vector3s
{
    public float x;
    public float y;
    public float z;

    public Vector3s()
    {
        x = y = z = 0;
    }

    public Vector3s(Vector3s clone)
    {
        x = clone.x;
        y = clone.y;
        z = clone.z;
    }

    public Vector3s(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public Vector3s(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public static Vector3s operator +(Vector3s a, Vector3s b)
    {
        return new Vector3s(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3s operator -(Vector3s a, Vector3s b)
    {
        return new Vector3s(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static bool operator ==(Vector3s a, Vector3s b)
    {
        return (a.x == b.x && a.y == b.y && a.z == b.z);
    }

    public static bool operator !=(Vector3s a, Vector3s b)
    {
        return !(a.x == b.x && a.y == b.y && a.z == b.z);
    }

    public static Vector3s operator *(Vector3s a, int b)
    {
        return new Vector3s(a.x * (float)b, a.y * (float)b, a.z * (float)b);
    }

    public static Vector3s operator *(Vector3s a, float b)
    {
        return new Vector3s(a.x * b, a.y * b, a.z * b);
    }
}

[System.Serializable]
public class Vector2s
{
    public float x;
    public float y;

    public Vector2s()
    {
        x = y = 0;
    }

    public Vector2s(Vector2s clone)
    {
        x = clone.x;
        y = clone.y;
    }

    public Vector2s(int _x, int _z)
    {
        x = _x;
        y = _z;
    }

    public Vector2s(float _x, float _z)
    {
        x = _x;
        y = _z;
    }

    public static Vector2s operator +(Vector2s a, Vector2s b)
    {
        return new Vector2s(a.x + b.x, a.y + b.y);
    }

    public static Vector2s operator -(Vector2s a, Vector2s b)
    {
        return new Vector2s(a.x - b.x, a.y - b.y);
    }

    public static bool operator ==(Vector2s a, Vector2s b)
    {
        return (a.x == b.x && a.y == b.y);
    }

    public static bool operator !=(Vector2s a, Vector2s b)
    {
        return !(a.x == b.x && a.y == b.y);
    }

    public static Vector2s operator *(Vector2s a, int b)
    {
        return new Vector2s(a.x * (float)b, a.y * (float)b);
    }

    public static Vector2s operator *(Vector2s a, float b)
    {
        return new Vector2s(a.x * b, a.y * b);
    }
}

[System.Serializable]
public class Colors
{
    public float r;
    public float g;
    public float b;
    public float a;

    public Colors()
    {
        r = g = b = a = 0;
    }

    public Colors(Colors clone)
    {
        r = clone.r;
        g = clone.g;
        b = clone.b;
        a = clone.a;
    }

    public Colors(int _r, int _g, int _b, int _a)
    {
        r = _r;
        g = _g;
        b = _b;
        a = _a;
    }

    public Colors(float _r, float _g, float _b, float _a)
    {
        r = _r;
        g = _g;
        b = _b;
        a = _a;
    }
}