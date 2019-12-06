using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public Transform item;
    public Transform shadow;
    public Basic basic;
    public World world;

    public BlockType _type = BlockType.Air;

    int vertexIndex = 0;
    List<Vector3s> vertices = new List<Vector3s>();
    List<int> triangles = new List<int>();

    public float count = 0;
    float y0;

    public float width = 0.1f;
    public float eight = 0.1f;

    public float gravity = -9.8f;
    public Vector3 velocity;
    private float verticalMomentum = 0;

    // Use this for initialization
    void Start () {
        y0 = item.position.y;
        world = GameObject.Find("World").GetComponent<World>();
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
        item.position = new Vector3(item.position.x,  y0 + 0.1f * Mathf.Sin(3f * Time.time), item.position.z);
        float size = 0.5f * (2 - item.localPosition.y) / 2f;
        if (size < 0)
            size = 0;
        shadow.localScale = new Vector3(size, size, 1f);


        CalculateVelocity();
        transform.Translate(velocity, Space.World);
    }

    private void CalculateVelocity()
    {
        // Affect vertical momentum with gravity.
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        if ((velocity.z > 0 && Control.front(world, transform.position, width)) || (velocity.z < 0 && Control.back(world, transform.position, width)))
            velocity.z = 0;
        if ((velocity.x > 0 && Control.right(world, transform.position, width)) || (velocity.x < 0 && Control.left(world, transform.position, width)))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = Control.checkDownSpeed(world, transform.position, velocity.y, width);
        else if (velocity.y > 0)
            velocity.y = Control.checkUpSpeed(world, transform.position, velocity.y, width);
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
}
