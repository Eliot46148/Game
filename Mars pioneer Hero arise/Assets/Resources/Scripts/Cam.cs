using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{

    public float rotationSpeed = 2.0f;
    public Vector3 position;
    public int sight = 5;

    void Start()
    {
        position = transform.position;
    }
    void Update()
    {
        float v = rotationSpeed * Input.GetAxis("Mouse Y");
        transform.Rotate(-v, 0, 0);
        /*
        
        if (transform.position != position)
        {
            Collider[] gameObjects = Physics.OverlapSphere(position, (float)sight);
            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].transform.parent.gameObject.SetActive(true);
            }
        }
        if(Vector3.Distance(transform.position, position) > 5)
            position = transform.position;*/
    }
}
