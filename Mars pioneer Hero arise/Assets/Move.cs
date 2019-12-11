using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    public Animator anim;

    float angle;

    Quaternion targetrotation;

    void Start()
    {

    }
    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            /*moveDirection = new Vector3(h, 0, v);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;*/
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
            CalculateDirection(h, v);
            Rotate();
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(transform.forward*speed);
        anim.SetInteger("state", 1);

        



        /*if(GameObject.Find("Minecraft").transform.position.y < -23)
        {
            GameObject.Find("Minecraft").transform.position = new Vector3(18.19f, 2.85f, 4.23f);
        }*/
    }

    void CalculateDirection(float h, float v)
    {
        angle = Mathf.Atan2(v, h);
        angle = Mathf.Rad2Deg * angle;
    }

    void Rotate()
    {
        targetrotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, 0.5f);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("SSS");
        if(hit.gameObject.tag == "moon")
        {
            GameObject.Find("Minecraft").transform.position = new Vector3(18.19f, 2.85f, 4.23f);
            //Invoke("SetPosition",1f);
        }
    }
    /*void SetPosition()
    {
        GameObject.Find("Minecraft").transform.position = new Vector3(18.19f, 2.85f, 4.23f);
    }*/
}
