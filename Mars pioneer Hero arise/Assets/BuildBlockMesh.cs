using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBlockMesh : MonoBehaviour
{
    public GameObject newBlock;
    public Basic.BlockType currentBlock;

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

        /*
        Vector2[] newMeshUVs = new Vector2[oldMeshUVs.Length + 24];

        for (int i = 0; i < oldMeshUVs.Length; i++)
            newMeshUVs[i] = oldMeshUVs[i];

        UV suv = block.GetComponent<UV>();
        float tileCol = 1 / suv.cols;
        float tileRow = 1 / suv.rows;

        float ui = tileRow * suv.tileX;
        float ua = tileRow * (suv.tileX + 1);
        float vi = tileCol * suv.tileY;
        float va = tileCol * (suv.tileY + 1);
        
        Vector2[] tempMeshUVs = suv.GetNewUVs(ui, ua, vi, va);
        for (int i = 0; i<24; i--)
            newMeshUVs[newMeshUVs.Length - 24 + i] = tempMeshUVs[i];

        transform.GetComponent<MeshFilter>().mesh.uv = newMeshUVs;*/

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
        block.GetComponent<UV>().SetTexture(blockType);
    }


    void Update()
    {
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

                CreateBlock(blockPos, currentBlock);/*
                GameObject block = (GameObject)Instantiate(newBlock, blockPos, Quaternion.identity);
                block.GetComponent<MeshRenderer>().material = Resources.Load("Material/Texture", typeof(Material)) as Material;
                block.AddComponent<UV>();
                block.GetComponent<UV>().SetTexture(Basic.BlockType.Sand);*/
                //block.transform.parent = this.transform;
                //Combine(block);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {

        }
    }
}
