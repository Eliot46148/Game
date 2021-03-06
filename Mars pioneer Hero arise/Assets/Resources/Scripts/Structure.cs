﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure
{
    public static Queue<VoxelMod> GenerateMajorFlora(int index, Vector3 position, int minTrunkHeight, int maxTrunkHeight)
    {
        switch (index)
        {
            case 0:
                return MakeTree(position, minTrunkHeight, maxTrunkHeight, BlockType.OakLog, BlockType.OakLeaves);
            case 1:
                return MakeCactus(position, minTrunkHeight, maxTrunkHeight);
        }
        return new Queue<VoxelMod>();
    }

    public static Queue<VoxelMod> MakeTree(Vector3 position, int minTrunkHeight, int maxTrunkHeight, BlockType trunkID = BlockType.OakLog, BlockType leafID = BlockType.OakLeaves)
    {
        Queue<VoxelMod> queue = new Queue<VoxelMod>();
        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));

        if (height < minTrunkHeight)
            height = minTrunkHeight;

        for (int i = 1; i < height; i++)
            queue.Enqueue(new VoxelMod(new Vector3s(position.x, position.y + i, position.z), trunkID));

        queue.Enqueue(new VoxelMod(new Vector3s(position.x, position.y + height, position.z), leafID));
        return queue;
    }

    public static Queue<VoxelMod> MakeCactus(Vector3 position, int minTrunkHeight, int maxTrunkHeight)
    {
        Queue<VoxelMod> queue = new Queue<VoxelMod>();

        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 23456f, 2f));

        if (height < minTrunkHeight)
            height = minTrunkHeight;

        for (int i = 1; i <= height; i++)
            queue.Enqueue(new VoxelMod(new Vector3s(position.x, position.y + i, position.z), BlockType.Cactus));

        return queue;
    }
}
