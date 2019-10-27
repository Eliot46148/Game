using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int seed;

    public Transform plater;
    public Vector3 spawnPosotopn;

    public Material texture;

    public int tileX;
    public int tileY;
    void Start () {
        int width = 100, height = 100;
        float scale = 20.0f;
        Vector3 position = transform.position;
        double[,] map = TerrainGeneration.CreateMap(width, height, position.x, position.z);

        GenerateWorld();

        /*for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                bool first = true;
                for (int k = (int)(map[i, j] * scale) - 1; k >= 0; k--)
                {
                    Vector3 blockPos = new Vector3(-width / 2 + i, k, -height / 2 + j);
                    if (first)
                    {
                        chunk = new Chunk(new ChunkCoord(0,0), this, Basic.BlockType.Grass, blockPos);
                        first = false;
                    }
                    else
                        chunk = new Chunk(this, Basic.BlockType.Dirt, blockPos);
                }
            }
        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(position.x, (float)map[(int)position.x, (int)position.z] * scale, position.z);*/
	}

    void GenerateWorld()
    {
        for (int x = 0; x < Voxel.worldSizeChunk; x++)
        {
            for (int z = 0; z < Voxel.worldSizeChunk; z++)
            {
                CreateNewChunk(x, z);
            }
        }
    }

    void CreateNewChunk(int x, int z)
    {
        Chunk chunk = new Chunk(new ChunkCoord(x, z), this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
