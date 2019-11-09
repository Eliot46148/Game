using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Air, Stone, Grass, Dirt, Cobblestone, Wooden, Bedrock, Water, Lava, Sand, Gravel, GoldOre, IronOre, CoalOre
}
[CreateAssetMenu(fileName = "BlockAssets", menuName = "Minecraft/BlockAsset")]
public class Basic : ScriptableObject
{
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

[System.Serializable]
public class BasicBlock
{
    public string name;
    public Vector2 icon;
    public BlockType type;
    public bool isSolid;
    public BlockTexture blockTexture;
    public BasicBlock(BlockType t, Vector2 i, BlockTexture texture, bool solid=true)
    {
        type = t;
        icon = i;
        blockTexture = texture;
        isSolid = solid;
    }
}

[System.Serializable]
public class BlockTexture
{
    public Vector2 side, plane, under;
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
