using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UV : MonoBehaviour {

    public float cols = 16;
    public float rows = 16;
    float tileCol = 0;
    float tileRow = 0;
    public Basic.BlockType blockType;
    private Basic basic = new Basic();

    public Vector2[] GetNewUVs(Basic.BlockType blockType)
    {
        Vector2[] blockUVs = new Vector2[24];
        List<Vector2> side = GetSideUVs(basic.textures[(int)blockType].Side);
        List<Vector2> plane = GetSideUVs(basic.textures[(int)blockType].Plane);
        List<Vector2> under = GetSideUVs(basic.textures[(int)blockType].Under);
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
                16, 17, 18, 19
            },
            new List<int>
            {
                23, 21, 20, 22
            },
            new List<int>
            {
                4, 5, 8, 9
            },
            new List<int>
            {
                15, 13 ,12, 14
            }
        };

        for (int i = 0; i < 6; i++)
            for (int j = 0; j < 4; j++)
                blockUVs[list[i][j]] = sides[i][j];

        return blockUVs;
    }

    private List<Vector2> GetSideUVs(Vector2 vector)
    {
        float i = vector.x;
        float j = vector.y;
        float ui = tileRow * i;
        float ua = tileRow * (i + 1);
        float vi = tileCol * (rows - j - 1);
        float va = tileCol * (rows - j);
        return new List<Vector2>
        {
            new Vector2(ui, vi),
            new Vector2(ui, va),
            new Vector2(ua, vi),
            new Vector2(ua, va)
        };
    }

    void Start ()
    {
        tileCol = 1 / cols;
        tileRow = 1 / rows;
        SetTexture(Basic.BlockType.Dirt);
	}

    public void SetTexture(Basic.BlockType blockType)
    {
        this.blockType = blockType;
        this.GetComponent<MeshFilter>().mesh.uv = GetNewUVs(blockType);
    }

    void Update()
    {
         
    }
}
