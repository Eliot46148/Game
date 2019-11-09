using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UV : MonoBehaviour
{

    public float cols = 24;
    public float rows = 82;
    float tileCol = 0;
    float tileRow = 0;
    public Basic basic;
    public BlockType blockType;

    public BlockType BlockType {
        get
        {
            return blockType;
        }
        set
        {
            blockType = value;
        }
    }

    public Vector2[] GetNewUVs(BlockType blockType)
    {
        Vector2[] blockUVs = new Vector2[24];
        BlockTexture texture = basic.Blocks[(int)blockType].blockTexture;
        List<Vector2> side = GetSideUVs(texture.Side);
        List<Vector2> plane = GetSideUVs(texture.Plane);
        List<Vector2> under = GetSideUVs(texture.Under);
        List<List<Vector2>> sides = new List<List<Vector2>>
        {
            side, side, side, side, plane, under
        };
        List<List<int>> list = new List<List<int>>
        {
            new List<int>
            {
                0, 2, 1, 3
            },
            new List<int>
            {
                7, 11, 6, 10
            },
            new List<int>
            {
                16, 17, 19, 18
            },
            new List<int>
            {
                20, 21, 23, 22
            },
            new List<int>
            {
                5, 9, 4, 8
            },
            new List<int>
            {
                14, 15, 13, 12
            }
        };

        for (int i = 0; i < 6; i++)
            for (int j = 0; j < 4; j++)
                blockUVs[list[i][j]] = sides[i][j];

        return blockUVs;
    }

    private List<Vector2> GetSideUVs(Vector2 vector)
    {
        float i = vector.y;
        float j = vector.x;
        float ui = tileCol * i;
        float ua = tileCol * (i + 1);
        float vi = tileRow * (rows - j - 1);
        float va = tileRow * (rows - j);
        return new List<Vector2>
        {
            new Vector2(ui, vi),
            new Vector2(ui, va),
            new Vector2(ua, vi),
            new Vector2(ua, va)
        };
    }

    void Start()
    {
        SetTexture(BlockType);
    }

    public void SetTexture(BlockType type)
    {
        BlockType = type;
        tileCol = 1 / cols;
        tileRow = 1 / rows;
        this.GetComponent<MeshFilter>().mesh.uv = GetNewUVs(type);
    }

    void Update()
    {

    }
}
