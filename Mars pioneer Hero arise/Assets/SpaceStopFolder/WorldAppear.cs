using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAppear : MonoBehaviour
{
    public GameObject canvas;
    public bool canvasappear = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerStay(UnityEngine.Collider NPC)
    {
        if (NPC.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!canvasappear)
                {
                    canvasappear = true;
                    canvas.SetActive(true);
                }
                else
                {
                    canvasappear = false;
                    canvas.SetActive(false);
                }
            }
        }
    }
}
