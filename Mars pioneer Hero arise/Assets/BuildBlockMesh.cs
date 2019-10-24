using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildBlockMesh : MonoBehaviour
{
    public GameObject newBlock;
    public Basic.BlockType currentBlock;
    private GameObject toBeDestroy;
    private float time;

    void Start()
    {
        int width = 100, height = 100;
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                Vector3 blockPos = new Vector3(-width/2 + i, 0, -height/2 + j);
                CreateBlock(blockPos, Basic.BlockType.Grass);
            }
    }

    void Combine(GameObject block)
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        Destroy(this.gameObject.GetComponent<MeshCollider>());

        Vector2[] oldMeshUVs = transform.GetComponent<MeshFilter>().mesh.uv;

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);

        transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        this.gameObject.AddComponent<MeshCollider>();
        transform.gameObject.SetActive(true);

        Destroy(block);
    }

    void CreateBlock(Vector3 position, Basic.BlockType blockType)
    {
        GameObject block = (GameObject)Instantiate(newBlock, position, Quaternion.identity);
        block.GetComponent<MeshRenderer>().material = Resources.Load("Material/Texture", typeof(Material)) as Material;
        block.AddComponent<UV>();
        block.GetComponent<UV>().BlockType = blockType;
        block.tag = "Block";
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 blockPos = hit.point + hit.normal / 2.0f;

                blockPos.x = (float)Mathf.Round(blockPos.x);
                blockPos.y = (float)Mathf.Round(blockPos.y);
                blockPos.z = (float)Mathf.Round(blockPos.z);
                
                CreateBlock(blockPos, currentBlock);
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                if (hit.collider.gameObject.tag == ("Block"))
                    toBeDestroy = hit.collider.gameObject;
            }
        }

        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            currentBlock--;
            if ((int)currentBlock < 0)
                currentBlock = Basic.BlockType.CoalOre;
            GameObject.Find("Text").GetComponent<Text>().text = currentBlock.ToString() + " (滾輪控制)";
        }
        else if (d < 0f)
        {
            currentBlock++;
            if ((int)currentBlock > (int)Basic.BlockType.CoalOre)
                currentBlock = 0;
            GameObject.Find("Text").GetComponent<Text>().text = currentBlock.ToString() + " (滾輪控制)";
        }

        if (toBeDestroy != null)
            time += Time.deltaTime;
        if (time > 0.1)
        {
            Destroy(toBeDestroy);
            time = 0;
        }
    }
}
