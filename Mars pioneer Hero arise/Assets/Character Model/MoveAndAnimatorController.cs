using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAndAnimatorController : MonoBehaviour {

    public float speed = 4f;
    public float gravity = 8f;

    Vector3 moveDir = Vector3.zero;

    CharacterController controller;
    Animator anim;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
            if (Input.GetKeyDown(KeyCode.W))
            {
                moveDir = new Vector3(0, 0, speed);
                anim.SetInteger("state", 1); 
                Debug.Log("!!!!");
            } 
    }
}
