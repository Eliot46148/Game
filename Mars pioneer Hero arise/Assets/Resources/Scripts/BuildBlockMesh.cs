using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildBlockMesh : MonoBehaviour
{
    public GameObject blockPrefab;
    private int currentBlock = 0;
    private GameObject toBeDestroy;
    private float time;
    private List<BasicBlock> itemsBar = new List<BasicBlock>
        {
            Basic.Blocks[(int)Basic.BlockType.Grass],
            Basic.Blocks[(int)Basic.BlockType.Dirt],
            Basic.Blocks[(int)Basic.BlockType.Stone],
            Basic.Blocks[(int)Basic.BlockType.Sand],
            Basic.Blocks[(int)Basic.BlockType.Gravel],
            Basic.Blocks[(int)Basic.BlockType.Cobblestone],
            Basic.Blocks[(int)Basic.BlockType.GoldOre],
            Basic.Blocks[(int)Basic.BlockType.Water],
            Basic.Blocks[(int)Basic.BlockType.Lava]
        };

    public List<BasicBlock> ItemsBar
    {
        get
        {
            return itemsBar;
        }

        set
        {
            itemsBar = value;
        }
    }

    public int CurrentBlock
    {
        get
        {
            return currentBlock;
        }

        set
        {
            currentBlock = value;
        }
    }

    void Start()
    {
        ItemsBar = new List<BasicBlock>
        {
            Basic.Blocks[(int)Basic.BlockType.Grass],
            Basic.Blocks[(int)Basic.BlockType.Dirt],
            Basic.Blocks[(int)Basic.BlockType.Stone],
            Basic.Blocks[(int)Basic.BlockType.Sand],
            Basic.Blocks[(int)Basic.BlockType.Gravel],
            Basic.Blocks[(int)Basic.BlockType.Cobblestone],
            Basic.Blocks[(int)Basic.BlockType.GoldOre],
            Basic.Blocks[(int)Basic.BlockType.Water],
            Basic.Blocks[(int)Basic.BlockType.Lava]
        };

        int width = 20, height = 20;
        float scale = 30;
        Vector3 position = transform.position;
        double[,] map = TerrainGeneration.CreateMap(width, height, position.x, position.z);
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                bool first = true;
                for (int k = (int)(map[i, j] * scale) - 1; k >= 0; k--)
                {
                    Vector3 blockPos = new Vector3(-width / 2 + i, k, -height / 2 + j);
                    if (first)
                    {
                        Basic.CreateBlock(blockPrefab, blockPos, Basic.Blocks[(int)Basic.BlockType.Grass]);
                        first = false;
                    }
                    else
                        Basic.CreateBlock(blockPrefab, blockPos, Basic.Blocks[(int)Basic.BlockType.Dirt]);
                }
            }
        transform.position = new Vector3(position.x, (float)map[(int)position.x, (int)position.z] * scale, position.z);
    }

    /*
    void Combine(GameObject block, Basic.BlockType blockType)
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        Destroy(this.gameObject.GetComponent<MeshCollider>());

        UV suv = block.GetComponent<UV>();
        Vector2[] oldMeshUVs = transform.GetComponent<MeshFilter>().mesh.uv, newTempMeshUVs = suv.GetNewUVs(blockType);

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);

        Vector2[] newMeshUVs = new Vector2[oldMeshUVs.Length + 24];
        oldMeshUVs.CopyTo(newMeshUVs, 0);
        newTempMeshUVs.CopyTo(newMeshUVs, 24);

        transform.GetComponent<MeshFilter>().mesh.uv = newMeshUVs;
        transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        this.gameObject.AddComponent<MeshCollider>();
        transform.gameObject.SetActive(true);

        Destroy(block);
    }*/

    void Update()
    {
        // Create cube
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 blockPos = hit.point + hit.normal / 2.0f;

                blockPos.x = (float)Mathf.Round(blockPos.x);
                blockPos.y = (float)Mathf.Round(blockPos.y);
                blockPos.z = (float)Mathf.Round(blockPos.z);
                
                Basic.CreateBlock(blockPrefab, blockPos, ItemsBar[CurrentBlock]);
            }
        }


        // Destroy cube
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                if (hit.collider.gameObject.tag == ("Block"))
                    toBeDestroy = hit.collider.gameObject;
            }
        }
        if (toBeDestroy != null)
            time += Time.deltaTime;
        if (time > 0.1)
        {
            Destroy(toBeDestroy);
            time = 0;
        }

        // Chose cube
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0.15f)
        {
            CurrentBlock--;
            if ((int)CurrentBlock < 0)
                CurrentBlock = 0;
        }
        else if (d < -0.15f)
        {
            CurrentBlock++;
            if ((int)CurrentBlock > 8)
                CurrentBlock = 8;
        }
    }
}
