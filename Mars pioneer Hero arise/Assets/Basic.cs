using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Basic
{
    public enum BlockType
    {
        Air, Stone, Grass, Dirt, Cobblestone, Wooden, b0, Bedrock, Water, b1, Lava, b2, Sand, Gravel, GoldOre, IronOre, CoalOre
    }

    public List<BlockTexture> textures = new List<BlockTexture> {
        new BlockTexture(new Vector2(0,0)),
        new BlockTexture(new Vector2(0,1)),
        new BlockTexture(new Vector2(0,3), new Vector2(0,0), new Vector2(0,2)),
        new BlockTexture(new Vector2(0,2)),
        new BlockTexture(new Vector2(1,0)),
        new BlockTexture(new Vector2(0,4)),
        new BlockTexture(new Vector2(0,0)),
        new BlockTexture(new Vector2(1,1)),
        new BlockTexture(new Vector2(0,0)),
        new BlockTexture(new Vector2(0,0)),
        new BlockTexture(new Vector2(14,13)),
        new BlockTexture(new Vector2(0,0)),
        new BlockTexture(new Vector2(1,2)),
        new BlockTexture(new Vector2(1,3)),
        new BlockTexture(new Vector2(2,0)),
        new BlockTexture(new Vector2(2,1)),
        new BlockTexture(new Vector2(2,2)),
        new BlockTexture(new Vector2(0,0)),
    };

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
