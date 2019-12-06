using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public float movementSpeed = 6.0f;//小球運動的速率
    private Vector3 horizontalMovement;//小球的水平運動
    //這裡理解為小球的前後運動
    private Vector3 verticalMovement;
    public Rigidbody rigidbody;


    float force = 150;
    bool isJump = false;
    void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal") * Vector3.right * movementSpeed;
        verticalMovement = Input.GetAxis("Vertical") * Vector3.forward * movementSpeed;
        //小球的運動（水平運動與前後運動的向量和）
        Vector3 movement = horizontalMovement + verticalMovement;
        //為小球施加力
        rigidbody.AddForce(movement, ForceMode.Force);
        if (Input.GetButton("Jump"))
        {
            if (!isJump)//如果还在跳跃中，则不重复执行 
            {
                rigidbody.AddForce(Vector3.up * force);
                isJump = true;
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "ground")//碰撞的是Plane  
        {
            isJump = false;
        }
    }

}
