using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;

    GameObject chunkObject;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    World world;

    byte[,,] voxelMap = new byte[Voxel.ChunkWidth, Voxel.ChunkHeight, Voxel.ChunkWidth];

    public Chunk(ChunkCoord c, World w)
    {
        coord = c;
        world = w;
        chunkObject = new GameObject();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();
        meshRenderer.material = world.texture;

        chunkObject.transform.position = new Vector3(coord.x * Voxel.ChunkWidth, 0f, coord.z * Voxel.ChunkWidth);
        chunkObject.transform.SetParent(world.transform);

        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;
        chunkObject.tag = "Block";
        chunkObject.layer = 8;


        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();

    }

    void PopulateVoxelMap()
    {
        for (int y = 0; y < Voxel.ChunkHeight; y++)
        {
            for (int x = 0; x < Voxel.ChunkWidth; x++)
            {
                for (int z = 0; z < Voxel.ChunkWidth; z++)
                {
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
                }
            }
        }

    }

    void CreateMeshData()
    {

        for (int y = 0; y < Voxel.ChunkHeight; y++)
        {
            for (int x = 0; x < Voxel.ChunkWidth; x++)
            {
                for (int z = 0; z < Voxel.ChunkWidth; z++)
                {
                    if (world.basic.Blocks[(int)voxelMap[x,y,z]].isSolid)
                        AddVoxelDataToChunk(new Vector3(x, y, z), voxelMap[x, y, z]);
                }
            }
        }

    }

    bool IsVoxelInChunks(int x, int y, int z)
    {
        if (x < 0 || x > Voxel.ChunkWidth - 1 || y < 0 || y > Voxel.ChunkHeight - 1 || z < 0 || z > Voxel.ChunkWidth - 1)
            return false;
        else
            return true;
    }

    public bool isActive
    {
        get
        {
            return chunkObject.activeSelf;
        }
        set
        {
            chunkObject.SetActive(value);
        }
    }

    public Vector3 position
    {
        get
        {
            return chunkObject.transform.position;
        }
    }

    bool CheckVoxel(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunks(x, y, z))
            return world.basic.Blocks[(int)world.GetVoxel(pos + position)].isSolid;

        return world.basic.Blocks[(int)voxelMap[x, y, z]].isSolid;

    }

    void AddVoxelDataToChunk(Vector3 position, byte type)
    {
        Vector2[] sides = new Vector2[6]
        {
            world.basic.Blocks[type].blockTexture.Side,
            world.basic.Blocks[type].blockTexture.Side,
            world.basic.Blocks[type].blockTexture.Plane,
            world.basic.Blocks[type].blockTexture.Under,
            world.basic.Blocks[type].blockTexture.Side,
            world.basic.Blocks[type].blockTexture.Side
        };

        for (int p = 0; p < 6; p++)
            if (!CheckVoxel(position + Voxel.faceChecks[p]) && world.basic.Blocks[(int)type].isSolid)
            {
                for (int i = 0; i < 6; i++)
                {
                    int triangleIndex = Voxel.voxelTris[p, i];
                    vertices.Add(Voxel.voxelVerts[triangleIndex] + position);
                    triangles.Add(vertexIndex);
                    uvs.Add(new Vector2((Voxel.voxelUvs[i].x + sides[p].y) / world.tileX, (Voxel.voxelUvs[i].y + world.tileY - sides[p].x - 1) / world.tileY));
                    vertexIndex++;
                }
            }
    }

    void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

    }
}

public class ChunkCoord
{
    public int x;
    public int z;
    public ChunkCoord(int _x, int _z)
    {
        x = _x;
        z = _z;
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