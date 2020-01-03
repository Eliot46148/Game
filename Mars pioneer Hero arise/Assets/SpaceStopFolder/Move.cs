using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Move : MonoBehaviour
{
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    Animator anim;

    public bool canMove = true;

    void Start()
    {
        anim = GameObject.Find("Minecraft").GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(-Input.GetAxis("Horizontal"), 0, -Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(-moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
        }

        if (!canMove)
            moveDirection = Vector3.zero;

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        GameObject.Find("Minecraft").transform.rotation = Quaternion.LookRotation(new Vector3(controller.velocity.x, 0, controller.velocity.z), Vector3.up);

        if (controller.velocity.x + controller.velocity.z != 0)
            anim.SetInteger("state", 1); 
        else
            anim.SetInteger("state", 0);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("SSS");
        if (hit.gameObject.tag == "moon")
        {
            transform.position = new Vector3(18.19f, 2.85f, 4.23f);
        }
    }

}