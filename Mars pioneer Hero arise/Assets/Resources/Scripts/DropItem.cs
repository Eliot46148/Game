using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public Transform item;
    public Transform shadow;
    public Transform player;
    public Basic basic;
    public World world;

    public ItemController ItemController;

    public BlockType _type = BlockType.Air;

    int vertexIndex = 0;
    List<Vector3s> vertices = new List<Vector3s>();
    List<int> triangles = new List<int>();

    public float count = 0;
    float y0;

    public float width = 0.1f;
    public float height = 0.1f;

    public float gravity = -9.8f;
    public Vector3 velocity;
    private float verticalMomentum = 0;

    // Use this for initialization
    void Start () {
        y0 = item.localPosition.y;
        world = GameObject.Find("World").GetComponent<World>();
        player = GameObject.Find("Player").transform;
        ItemController = GameObject.Find("GameController").GetComponent<ItemController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        count += Time.deltaTime;
        if (count >= 300)
            Destroy(transform.gameObject);
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 75 * Time.deltaTime, 0));
        item.localPosition = new Vector3(item.localPosition.x,  y0 + 0.1f * Mathf.Sin(3f * Time.time), item.localPosition.z);
        float size = 0.5f * (2 - item.localPosition.y) / 2f;
        if (size < 0)
            size = 0;
        shadow.localScale = new Vector3(size, size, 1f);

        OnTrigger();

        transform.Translate(velocity, Space.World);
    }

    public void ChangeSkin(BlockType type)
    {
        _type = type;
        MeshFilter meshFilter = item.GetComponent<MeshFilter>();
        
        List<Vector2s> uvs = new List<Vector2s>();
        for (int p = 0; p < 6; p++)
        {
            Vector2s side = basic.Blocks[(int)_type].GetTextureID(p);
            float textureSizeCols = 1f / basic.tileCols;
            float textureSizeRows = 1f / basic.tileRows;
            float c1 = side.y / basic.tileCols;
            float r1 = side.x / basic.tileRows;
            r1 = 1f - r1 - textureSizeRows;
            float c2 = c1 + textureSizeCols;
            float r2 = r1 + textureSizeRows;

            for (int i = 0; i < 4; i++)
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);

            uvs.Add(new Vector2s(c1, r1));
            uvs.Add(new Vector2s(c1, r2));
            uvs.Add(new Vector2s(c2, r1));
            uvs.Add(new Vector2s(c2, r2));

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 3);
            vertexIndex += 4;
        }
        meshFilter.mesh.vertices = World.sl3vl(vertices).ToArray();
        meshFilter.mesh.SetTriangles(triangles.ToArray(), 0);
        meshFilter.mesh.uv = World.sl2vl(uvs).ToArray();
        meshFilter.mesh.RecalculateNormals();
    }

    private void OnTrigger()
    {
        Debug.DrawLine(transform.position, player.position, Color.green);
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < 0.3f)
        {
            if (ItemController.PickItem(new ItemStack((byte)_type, 1)))
                Destroy(transform.gameObject);
        }else if (distance < 2.5f)
        {
            transform.position += (player.position - transform.position) * 3f * Time.deltaTime;
        }
    }
}
