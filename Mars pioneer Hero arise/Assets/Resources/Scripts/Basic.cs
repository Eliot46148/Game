using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class Basic
{
    [System.Serializable]
    public enum BlockType
    {
        Air, Stone, Grass, Dirt, Cobblestone, Wooden, Bedrock = 7, Water, Lava = 10, Sand = 12, Gravel, GoldOre, IronOre, CoalOre
    }

    public static List<BasicBlock> Blocks = new List<BasicBlock> {
        new BasicBlock(BlockType.Air, new Vector2(30,14), new BlockTexture(new Vector2(0,0)), false),
        new BasicBlock(BlockType.Stone, new Vector2(0,0), new BlockTexture(new Vector2(0,1))),
        new BasicBlock(BlockType.Grass, new Vector2(0,1), new BlockTexture(new Vector2(0,3), new Vector2(0,0), new Vector2(0,2))),
        new BasicBlock(BlockType.Dirt, new Vector2(0,2), new BlockTexture(new Vector2(0,2))),
        new BasicBlock(BlockType.Cobblestone, new Vector2(0,3), new BlockTexture(new Vector2(1,0))),
        new BasicBlock(BlockType.Wooden, new Vector2(0,4), new BlockTexture(new Vector2(0,4))),
        new BasicBlock(BlockType.Air, new Vector2(0,0), new BlockTexture(new Vector2(0,0))),
        new BasicBlock(BlockType.Bedrock, new Vector2(0,12), new BlockTexture(new Vector2(1,1))),
        new BasicBlock(BlockType.Water, new Vector2(18,6), new BlockTexture(new Vector2(3,22)), false),
        new BasicBlock(BlockType.Air, new Vector2(0,0), new BlockTexture(new Vector2(0,0))),
        new BasicBlock(BlockType.Lava, new Vector2(18,7), new BlockTexture(new Vector2(3,21)), false),
        new BasicBlock(BlockType.Air, new Vector2(0,0), new BlockTexture(new Vector2(0,0))),
        new BasicBlock(BlockType.Sand, new Vector2(1,1), new BlockTexture(new Vector2(1,2))),
        new BasicBlock(BlockType.Gravel, new Vector2(1,2), new BlockTexture(new Vector2(1,3))),
        new BasicBlock(BlockType.GoldOre, new Vector2(1,3), new BlockTexture(new Vector2(2,0))),
        new BasicBlock(BlockType.IronOre, new Vector2(1,4), new BlockTexture(new Vector2(2,1))),
        new BasicBlock(BlockType.CoalOre, new Vector2(1,5), new BlockTexture(new Vector2(2,2)))
    };
    public static GameObject CreateBlock(GameObject blockPrefab, Vector3 position, BasicBlock newBlock)
    {
        GameObject block = (GameObject)GameObject.Instantiate(blockPrefab, position, Quaternion.identity);
        //block.SetActive(false);
        block.GetComponent<UV>().BlockType = newBlock.Type;
        block.transform.parent = GameObject.Find("World").transform;
        block.tag = "Block";
        block.layer = 8;
        return block;
    }
}

public class BasicBlock
{
    private Vector2 icon;
    private BlockTexture blockTexture;
    private Basic.BlockType type;
    private bool isSolid;
    public BasicBlock(Basic.BlockType t, Vector2 i, BlockTexture texture, bool solid=true)
    {
        Type = t;
        Icon = i;
        Texture = texture;
        IsSolid = solid;
    }

    public Vector2 Icon
    {
        get
        {
            return icon;
        }

        set
        {
            icon = value;
        }
    }

    public BlockTexture Texture
    {
        get
        {
            return blockTexture;
        }

        set
        {
            blockTexture = value;
        }
    }

    public Basic.BlockType Type
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

    public bool IsSolid
    {
        get
        {
            return isSolid;
        }

        set
        {
            isSolid = value;
        }
    }
}

public class BlockTexture
{
    private Vector2 side, plane, under;
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
