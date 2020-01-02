using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 方塊ID定義
public enum BlockType
{
    Air, Stone, Grass, Dirt, Cobblestone, Wooden, Bedrock, Water, Lava, Sand, Gravel, GoldOre, IronOre, CoalOre, Glass, OakLog, OakLeaves, Cactus, SnowGrass, Box
}

// 所有方塊設定
[CreateAssetMenu(fileName = "BlockAssets", menuName = "Minecraft/BlockAsset")]
public class Basic : ScriptableObject
{
    [Header("方塊矩陣大小定義")]
    public float tileRows;
    public float tileCols;
    [Header("方塊圖示矩陣大小定義")]
    public float rows;
    public float cols;
    [Header("(用Empty去複製新增方塊)")]
    public List<BasicBlock> Blocks;
}

// 基本方塊設定
[System.Serializable]
public class BasicBlock
{
    public string name;
    [Header("方塊圖示矩陣座標")]
    public Vector2s icon;
    public Sprite iconImage;
    public BlockType type;
    [Header("方塊是否有實體")]
    public bool isSolid;
    [Header("方塊是否不會擋住旁邊物體")]
    public bool renderNeighborFaces;
    [Header("方塊透明度 (0為不透明)")]
    public float transparency;
    [Header("方塊材質設定")]
    public BlockTexture blockTexture;

    public BasicBlock(BlockType t, Vector2s i, BlockTexture texture, bool solid = true)
    {
        type = t;
        icon = i;
        blockTexture = texture;
        isSolid = solid;
    }

    public Vector2s GetTextureID(int faceIndex)
    {

        switch (faceIndex)
        {

            case 0:
            case 1:
            case 4:
            case 5:
                return blockTexture.side;
            case 2:
                return blockTexture.plane;
            case 3:
                return blockTexture.under;
            default:
                return new Vector2s(0, 0);
        }
    }
}

[System.Serializable]
public class BlockTexture
{
    [Header("材質四邊座標")]
    public Vector2s side;
    [Header("材質頂面座標")]
    public Vector2s plane;
    [Header("材質底面座標")]
    public Vector2s under;
    public BlockTexture(Vector2s s)
    {
        Side = s;
        Plane = s;
        Under = s;
    }
    public BlockTexture(Vector2s s, Vector2s p)
    {
        Side = s;
        Under = p;
        Plane = p;
    }
    public BlockTexture(Vector2s s, Vector2s p, Vector2s u)
    {
        Side = s;
        Under = u;
        Plane = p;
    }

    public Vector2s Side
    {
        get
        {
            return side;
        }

        set
        {
            side = value;
        }
    }

    public Vector2s Plane
    {
        get
        {
            return plane;
        }

        set
        {
            plane = value;
        }
    }

    public Vector2s Under
    {
        get
        {
            return under;
        }

        set
        {
            under = value;
        }
    }
}

public class Item
{
    public Item(BlockType t, int n, bool i)
    {
        type = t;
        number = n;
        isblock = i;
    }
    public BlockType type;
    public int number;
    public bool isblock;

    public BlockType Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public int Number
    {
        get
        {
            return number;
        }

        set
        {
            number = value;
        }
    }

    public bool Isblock
    {
        get
        {
            return isblock;
        }

        set
        {
            isblock = value;
        }
    }

}

// Perlin 雜訊產生地圖的程序化生成
public static class Noise
{

    public static float Get2DPerlin(Vector2 position, float offset, float scale)
    {
        return Mathf.PerlinNoise((position.x + 0.1f) / VoxelData.ChunkWidth * scale + offset, (position.y + 0.1f) / VoxelData.ChunkWidth * scale + offset);
    }

    public static bool Get3DPerlin(Vector3 position, float offset, float scale, float threshold)
    {
        float x = (position.x + offset + 0.1f) * scale;
        float y = (position.y + offset + 0.1f) * scale;
        float z = (position.z + offset + 0.1f) * scale;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);
        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        if ((AB + BC + AC + BA + CB + CA) / 6f > threshold)
            return true;
        else
            return false;
    }
}

[System.Serializable]
public class WrappingClass
{
    public ChunkCoord Coordinate;
    public List<VoxelMod> Modifications;

    public WrappingClass(ChunkCoord c, List<VoxelMod> map)
    {
        Coordinate = c;
        Modifications = map;
    }
}

[System.Serializable]
public class SaveItem
{
    public int id;
    public int amount;

    public SaveItem(int ID, int AMOUNT)
    {
        id = ID;
        amount = AMOUNT;
    }
}

[System.Serializable]
public class SaveData
{
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    //public List<BlockType> items;
    public List<string> VoxelMaps;
    public List<string> Bag;
    public List<string> Toolbar;

    public SaveData(Vector3 position, Quaternion rotation, List<string> bag, List<string> toolbar, List<string> map)
    {
        PlayerPosition = position;
        PlayerRotation = rotation;
        Bag = bag;
        Toolbar = toolbar;
        VoxelMaps = map;
    }
}