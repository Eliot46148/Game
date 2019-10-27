using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public Camera minimap;
    private Vector3 moveDirection = Vector3.zero;

    void Start ()
	{
	}

	void FixedUpdate ()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (true)//controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"), 0);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetKey(KeyCode.Space))
                moveDirection.y = 4.0f;
            if (Input.GetKey(KeyCode.Q))
                moveDirection.y = -4.0f;
            /*if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;*/
        }
        //moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
        minimap.transform.position = new Vector3(transform.position.x, 200, transform.position.z);

        float h = 2.0f * Input.GetAxis("Mouse X");
        transform.Rotate(0, 0, h);
    }
}
