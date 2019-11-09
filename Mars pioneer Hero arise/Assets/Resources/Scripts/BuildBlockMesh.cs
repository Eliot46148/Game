using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildBlockMesh : MonoBehaviour
{
    public GameObject blockPrefab;
    private GameObject toBeDestroy;
    private float time;
    public Basic basic;

    public Backpack backpack;

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

                basic.CreateBlock(blockPrefab, blockPos, backpack.ItemsBar[backpack.CurrentBlock]);
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
            backpack.CurrentBlock--;
            if ((int)backpack.CurrentBlock < 0)
                backpack.CurrentBlock = 0;
        }
        else if (d < -0.15f)
        {
            backpack.CurrentBlock++;
            if ((int)backpack.CurrentBlock > 8)
                backpack.CurrentBlock = 8;
        }
    }
}
