using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgent : MonoBehaviour
{
    public float stoppingDistance;
    public Vector3 destination;

    public Collider collider;

    public float walkSpeed = 3f;
    public float jumpForce = 3f;

    public bool walking = false;

    // Start is called before the first frame update
    void Start()
    {
        collider = transform.GetComponent<Collider>();
        destination = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int x = (int)(destination.x - transform.position.x);
        int z = (int)(destination.z - transform.position.z);
        float distance = Vector3.Distance(destination, transform.position);
        x = (x > stoppingDistance ? 1 : x < -stoppingDistance ? -1 : 0);
        z = (z > stoppingDistance ? 1 : z < -stoppingDistance ? -1 : 0);
        if (x == 0 && z == 0)
            walking = false;
        if (walking)
        {
            collider.velocity = (new Vector3(x, 0, z) * Time.fixedDeltaTime * walkSpeed);

            collider.CalculateVelocity();

            if (z != 0 && distance > stoppingDistance && collider.velocity.z == 0 && collider.isAbleToUp && collider.isGround)
                collider.verticalMomentum = jumpForce;
        }
    }

    public void SetDestination(Vector3 position)
    {
        destination = position;
        walking = true;
    }

    // Show the lookRadius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}
