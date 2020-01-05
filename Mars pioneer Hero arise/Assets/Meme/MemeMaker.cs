using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemeMaker : MonoBehaviour
{
    public Texture2D meme;
    public Transform parent;
    public GameObject blockPrefab;

    public GameObject debugPlanePrefab;

    World world;
    bool isEnable = false;
    List<VoxelMod> mods = new List<VoxelMod>();

    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isEnable)
            {
                foreach (Transform child in parent)
                    Destroy(child.gameObject);

                parent.gameObject.SetActive(false);
                isEnable = false;
            }
            else
            {
                StartCoroutine(Making(32));
                parent.gameObject.SetActive(true);
                isEnable = true;
            }
        }
    }

    IEnumerator Making(int resolution = 1)
    {
        StartCoroutine(Convert(transform.position, resolution));
        Debug.Log("Done");
        /*
        foreach (VoxelMod mod in mods)
        {
            Debug.Log(mod.id);
            Debug.Log(World.sv(mod.position));
            //world.GetChunkFromVector3s(mod.position).EditVoxel(World.sv(mod.position), mod.id);
        }
        */
        yield return null;
    }


    IEnumerator Convert(Vector3 pos, int resolution = 1)
    {
        mods.Clear();

        List<BasicBlock> choices = new List<BasicBlock>();
        List<Color32> colors = new List<Color32>();
        Color32[] texColors = meme.GetPixels32();

        foreach (BasicBlock block in world.blocks)
            if (block.name.Contains("Wool"))
            {
                choices.Add(block);
                Color32 color = GetWallAverageColor(block);
                colors.Add(color);
                Debug.Log(block.type.ToString() + " color : " + color.ToString());
            }

        for (int y = 0; y <= meme.height - resolution; y+= resolution)
        {
            for (int x = 0; x <= meme.width - resolution; x+= resolution)
            {
                int width = ((x + resolution) > meme.width ? meme.width - x : resolution);
                int height = ((y + resolution) > meme.height ? meme.height - y : resolution);
                //List<Color32> med = new List<Color32>();
                Texture2D tex = new Texture2D(width, height);
                for (int b = 0; b < height; b++)
                    for (int a = 0; a < width; a++)
                /*    {
                        Color temp = texColors[(x + a) + (y + b) * meme.width];
                        med.Add(temp);
                    }
                */tex.SetPixel(a, b, texColors[(x + a) + (y + b) * meme.width]);

                Color32 average = AverageColorFromTexture(tex);
                //Color32 average = med[med.Count / 2];

                float distance = Vector3.Distance(new Vector3(average.r, average.g, average.b), Vector3.zero);
                BlockType id = BlockType.Wool1;
                //BlockType id = choices[closestColor(colors, average)].type;
                
                for (int i = 0; i < choices.Count; i++)
                {
                    BasicBlock block  = choices[i];
                    Color32 comp = colors[i];
                    float dis = Vector3.Distance(new Vector3(average.r, average.g, average.b), new Vector3(comp.r, comp.g, comp.b));
                    if (dis < distance)
                    {
                        distance = dis;
                        id = block.type;
                    }
                }
                
                Vector3 position = pos + new Vector3(x / resolution, y / resolution, 0);
                mods.Add(new VoxelMod(World.vs(position), id));
                world.GetChunkFromVector3s(World.vs(position)).EditVoxel(position, id);
            }
        }
        yield return null;
    }

    // weighed distance using hue, saturation and brightness
    int closestColor(List<Color32> colors, Color32 target)
    {
        float h, s, v;
        Color.RGBToHSV(target, out h, out s, out v);

        var num1 = ColorNum(target);

        List<float> diffs = new List<float>();
        foreach (Color color in colors)
        {
            float hh;
            Color.RGBToHSV(target, out hh, out s, out v);
            diffs.Add(Mathf.Abs(ColorNum(color) - num1) + getHueDistance(hh, h));

        }

        var diffMin = diffs.Min(x => x);
        return diffs.ToList().FindIndex(n => n == diffMin);
    }
    float ColorNum(Color c)
    {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);

        return s * 100f +
                getBrightness(c) * 100f;
    }
    // color brightness as perceived:
    float getBrightness(Color c)
    { return (c.r * 0.299f + c.g * 0.587f + c.b * 0.114f) / 256f; }
    // distance between two hues:
    float getHueDistance(float hue1, float hue2)
    {
        float d = Mathf.Abs(hue1 - hue2); return d > 180 ? 360 - d : d;
    }

    Color32 GetWallAverageColor(BasicBlock block)
    {
        Color32 color;
        BlockTexture tex = block.blockTexture;
        Vector2 side = World.s2v(tex.side);
        Texture2D t = (Texture2D)(world.transparentMaterial.mainTexture);
        Color32[] texColors = t.GetPixels32();

        int width = (int)(t.width / world.basic.tileCols);
        int height = (int)(t.height / world.basic.tileRows);


        float textureSizeCols = 1f / world.basic.tileCols;
        float textureSizeRows = 1f / world.basic.tileRows;
        float c1 = side.y / world.basic.tileCols;
        float r1 = side.x / world.basic.tileRows;
        r1 = 1f - r1 - textureSizeRows;
        float c2 = c1 + textureSizeCols;
        float r2 = r1 + textureSizeRows;


        Texture2D texture = new Texture2D(width, height);
        for (int b = 0; b < height; b++)
            for (int a = 0; a < width; a++)
                texture.SetPixel(a, b, texColors[((int)(c1 * t.height) + a) + ((int)(r1 * t.height) + b) * t.width]);
        //GameObject obj =  Instantiate(debugPlanePrefab, parent);
        //texture.filterMode = FilterMode.Point;
        //obj.GetComponent<Renderer>().material.mainTexture = texture;

        return AverageColorFromTexture(texture);
    }


    Color32 AverageColorFromTexture(Texture2D tex)
    {

        Color32[] texColors = tex.GetPixels32();

        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        for (int i = 0; i < total; i++)
        {

            r += texColors[i].r;

            g += texColors[i].g;

            b += texColors[i].b;

        }

        return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 0);

    }
}
