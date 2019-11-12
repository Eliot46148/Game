using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 方塊ID定義
public enum BlockType
{
    Air, Stone, Grass, Dirt, Cobblestone, Wooden, Bedrock, Water, Lava, Sand, Gravel, GoldOre, IronOre, CoalOre, Glass, OakLog, OakLeaves, Cactus, SnowGrass
}

// 所有方塊設定
[CreateAssetMenu(fileName = "BlockAssets", menuName = "Minecraft/BlockAsset")]
public class Basic : ScriptableObject
{
    public int tileX;
    public int tileY;
    [Header("(用Empty去複製新增方塊)")]
    public List<BasicBlock> Blocks;
    public GameObject CreateBlock(GameObject blockPrefab, Vector3 position, BlockType newBlock)
    {
        GameObject block = (GameObject)GameObject.Instantiate(blockPrefab, position, Quaternion.identity);
        block.GetComponent<UV>().BlockType = newBlock;
        block.GetComponent<UV>().basic = this;
        block.transform.parent = GameObject.Find("World").transform;
        block.tag = "Block";
        block.layer = 8;
        return block;
    }
}

// 基本方塊設定
[System.Serializable]
public class BasicBlock
{
    public string name;
    [Header("方塊圖示矩陣座標")]
    public Vector2 icon;
    public BlockType type;
    [Header("方塊是否有實體")]
    public bool isSolid;
    [Header("方塊是否不會擋住旁邊物體")]
    public bool renderNeighborFaces;
    [Header("方塊透明度 (0為不透明)")]
    public float transparency;
    [Header("方塊材質設定")]
    public BlockTexture blockTexture;

    public BasicBlock(BlockType t, Vector2 i, BlockTexture texture, bool solid=true)
    {
        type = t;
        icon = i;
        blockTexture = texture;
        isSolid = solid;
    }

    public Vector2 GetTextureID(int faceIndex)
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
                Debug.Log("Error in GetTextureID; invalid face index");
                return Vector2.zero;
        }
    }
}

[System.Serializable]
public class BlockTexture
{
    [Header("材質四邊座標")]
    public Vector2 side;
    [Header("材質頂面座標")]
    public Vector2 plane;
    [Header("材質底面座標")]
    public Vector2 under;
    public BlockTexture(Vector2 s)
    {
        Side = s;
        Plane = s;
        Under = s;
    }
    public BlockTexture(Vector2 s, Vector2 p)
    {
        Side = s;
        Under = p;
        Plane = p;
    }
    public BlockTexture(Vector2 s, Vector2 p, Vector2 u)
    {
        Side = s;
        Under = u;
        Plane = p;
    }

    public Vector2 Side
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

    public Vector2 Plane
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

    public Vector2 Under
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
