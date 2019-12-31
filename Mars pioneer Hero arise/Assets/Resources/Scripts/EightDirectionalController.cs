using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EightDirectionalController : MonoBehaviour {

    public float velocity = 5;
    public float turnspeed = 10;

    Vector2 input;
    float angle;

    public bool attack;

    Quaternion targetrotation;
    Transform cam;

    Animator anim;

    private void Start()
    {
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInput();

        if (Mathf.Abs(input.x) < 1 && Mathf.Abs(input.y) < 1)
            anim.SetInteger("state", 0);
        else
        {
            anim.SetInteger("state", 1);
            CalculateDirection();
            Rotate();
            Move();
        }            
        anim.SetBool("attack", attack);
    }

    void GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        attack = Input.GetKey(KeyCode.Mouse0);  
    }

    void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;
    }

    void Rotate()
    {
        targetrotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, turnspeed*Time.deltaTime);
    }

    void Move()
    {
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
}
