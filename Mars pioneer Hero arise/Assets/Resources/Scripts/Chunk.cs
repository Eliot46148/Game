using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    World world;

    Basic.BlockType[,,] voxelMap = new Basic.BlockType[Voxel.ChunkWidth, Voxel.ChunkHeight, Voxel.ChunkWidth];

    public Chunk(ChunkCoord c, World w)
    {
        coord = c;
        world = w;
        GameObject chunkObject = new GameObject();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer.material = world.texture;

        chunkObject.transform.position = new Vector3(coord.x * Voxel.ChunkWidth, 0f, coord.x * Voxel.ChunkHeight);
        chunkObject.transform.parent = world.transform;

        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;
        chunkObject.tag = "Block";
        chunkObject.layer = 8;


        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();

    }

    void PopulateVoxelMap()
    {
        double[,] map = TerrainGeneration.CreateMap(Voxel.ChunkWidth, Voxel.ChunkHeight, coord.x + Voxel.ChunkWidth / 2, coord.z + Voxel.ChunkHeight / 2);
        for (int y = 0; y < Voxel.ChunkHeight; y++)
        {
            for (int x = 0; x < Voxel.ChunkWidth; x++)
            {
                for (int z = 0; z < Voxel.ChunkWidth; z++)
                {
                    if (y > (int)(map[x, z] * Voxel.ChunkHeight))
                        voxelMap[x, y, z] = Basic.BlockType.Air;
                    else if (y == 0)
                        voxelMap[x, y, z] = Basic.BlockType.Bedrock;
                    else if (y == (int)(map[x, z] * Voxel.ChunkHeight))
                        voxelMap[x, y, z] = Basic.BlockType.Grass;
                    else
                        voxelMap[x, y, z] = Basic.BlockType.Dirt;

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

    bool CheckVoxel(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunks(x, y, z))
            return false;

        return Basic.Blocks[(int)voxelMap[x, y, z]].IsSolid;

    }

    void AddVoxelDataToChunk(Vector3 position, Basic.BlockType type)
    {
        Vector2[] sides = new Vector2[6]
        {
            Basic.Blocks[(int)type].Texture.Side,
            Basic.Blocks[(int)type].Texture.Side,
            Basic.Blocks[(int)type].Texture.Plane,
            Basic.Blocks[(int)type].Texture.Under,
            Basic.Blocks[(int)type].Texture.Side,
            Basic.Blocks[(int)type].Texture.Side
        };

        for (int p = 0; p < 6; p++)
            if (!CheckVoxel(position + Voxel.faceChecks[p]) && Basic.Blocks[(int)type].IsSolid)
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
}