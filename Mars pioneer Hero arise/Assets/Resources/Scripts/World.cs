using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int seed;
    public BiomeAttribute biome;

    public Transform player;
    public Vector3 spawnPosition;

    public Basic basic;
    public Material texture;

    public int tileX;
    public int tileY;

    Chunk[,] chunks = new Chunk[Voxel.WorldSizeInChunks, Voxel.WorldSizeInChunks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    void Start () {
        Random.InitState(seed);

        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);
        if (!playerChunkCoord.Equals(playerLastChunkCoord))
        {
            CheckViewDistance();
            playerLastChunkCoord = playerChunkCoord;
        }
    }

    void GenerateWorld()
    {
        for (int x = (Voxel.WorldSizeInChunks / 2) - Voxel.ViewDistanceInChunks; x < (Voxel.WorldSizeInChunks / 2) + Voxel.ViewDistanceInChunks; x++)
        {
            for (int z = (Voxel.WorldSizeInChunks / 2) - Voxel.ViewDistanceInChunks; z < (Voxel.WorldSizeInChunks / 2) + Voxel.ViewDistanceInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }
        Vector3 pos = new Vector3((Voxel.WorldSizeInChunks * Voxel.ChunkWidth) / 2f, 0f, (Voxel.WorldSizeInChunks* Voxel.ChunkWidth) / 2f);
        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale) + biome.solidGroundHeight + 0.5f);
        spawnPosition = new Vector3((Voxel.WorldSizeInChunks * Voxel.ChunkWidth) / 2f, terrainHeight, (Voxel.WorldSizeInChunks * Voxel.ChunkWidth) / 2f);
        player.position = spawnPosition;
    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / Voxel.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / Voxel.ChunkWidth);
        return new ChunkCoord(x, z);
    }

    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        List<ChunkCoord> previousActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - Voxel.ViewDistanceInChunks; x < coord.x + Voxel.ViewDistanceInChunks; x++)
        {
            for (int z = coord.z - Voxel.ViewDistanceInChunks; z < coord.z + Voxel.ViewDistanceInChunks; z++)
            {
                if (isChunkInWorld(new ChunkCoord(x, z)))
                {
                    if (chunks[x, z] == null)
                        CreateNewChunk(x, z);
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(new ChunkCoord(x, z));
                    }
                }

                for (int i = 0; i < previousActiveChunks.Count; i++)
                    if (previousActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previousActiveChunks.RemoveAt(i);
            }
        }

        foreach (ChunkCoord c in previousActiveChunks)
            chunks[c.x, c.z].isActive = false;
    }

    public byte GetVoxel(Vector3 pos)
    {
        int yPos = Mathf.FloorToInt(pos.y);

        /* Outside the world, return Air */
        if (!isVoxelInWorld(pos))
            return (byte)BlockType.Air;

        /* Bottom of the world, return Bedrock */
        if (yPos == 0)
            return (byte)BlockType.Bedrock;

        /* Basic terrain pass */
        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale) + biome.solidGroundHeight);
        byte voxelValue = 0;

        if (yPos == terrainHeight)
            voxelValue = (byte)BlockType.Grass;
        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            voxelValue = (byte)BlockType.Dirt;
        else if (yPos > terrainHeight)
            return (byte)BlockType.Air;
        else
            voxelValue = (byte)BlockType.Stone;

        /* Second terrain pass */
        if (voxelValue == (byte)BlockType.Stone)
        {
            foreach(Lode lode in biome.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                    voxelValue = lode.blockID;
            }
        }
        return voxelValue;
    }

    void CreateNewChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunks.Add(new ChunkCoord(x, z));
    }

    bool isChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < Voxel.WorldSizeInChunks - 1 && coord.z > 0 && coord.z < Voxel.WorldSizeInChunks - 1)
            return true;
        else
            return false;
    }

    bool isVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < Voxel.WorldSizeInVoxels - 1 && pos.y >= 0 && pos.y < Voxel.ChunkHeight && pos.z >= 0 && pos.z < Voxel.WorldSizeInVoxels - 1)
            return true;
        else
            return false;
    }
}