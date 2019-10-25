using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildBlockMesh : MonoBehaviour
{
    public GameObject newBlock;
    private int currentBlock = 0;
    private GameObject toBeDestroy;
    private float time;
    private List<BasicBlock> itemsBar;

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
        Basic basic = new Basic();
        ItemsBar = new List<BasicBlock>
        {
            basic.Blocks[(int)Basic.BlockType.Grass],
            basic.Blocks[(int)Basic.BlockType.Dirt],
            basic.Blocks[(int)Basic.BlockType.Stone],
            basic.Blocks[(int)Basic.BlockType.Sand],
            basic.Blocks[(int)Basic.BlockType.Gravel],
            basic.Blocks[(int)Basic.BlockType.Cobblestone],
            basic.Blocks[(int)Basic.BlockType.GoldOre],
            basic.Blocks[(int)Basic.BlockType.Water],
            basic.Blocks[(int)Basic.BlockType.Lava]
        };

        int width = 100, height = 100;
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                Vector3 blockPos = new Vector3(-width/2 + i, 0, -height/2 + j);
                CreateBlock(blockPos, new Basic().Blocks[(int)Basic.BlockType.Grass]);
            }
    }

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
    }

    public GameObject CreateBlock(Vector3 position, BasicBlock blockPrefab)
    {
        GameObject block = (GameObject)Instantiate(newBlock, position, Quaternion.identity);
        //block.SetActive(false);
        block.GetComponent<UV>().BlockType = blockPrefab.Type;
        //block.transform.parent = this.transform;
        block.tag = "Block";
        block.layer = 8;
        return block;
        //Combine(block, blockType);
    }

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
                
                CreateBlock(blockPos, ItemsBar[CurrentBlock]);
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
