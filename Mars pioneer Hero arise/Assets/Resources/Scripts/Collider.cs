using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider : MonoBehaviour
{
    private World world;

    public float width = 1;
    public float depth = 1;
    public float height = 1;

    public float gravity = -15f;

    public bool isGround;

    public Vector3 velocity;
    public float verticalMomentum = 0;

    public bool isAbleToUp;
    public bool isAbleToDown;
    public bool isAbleToLeft;
    public bool isAbleToRight;
    public bool isAbleToFront;
    public bool isAbleToBack;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (world != null)
        {

            CalculateVelocity();

        }
        transform.Translate(velocity, Space.World);
        if (anim != null)
        {
            if (velocity.x!=0 || velocity.z != 0)
                anim.SetInteger("state", 1);
            else
                anim.SetInteger("state", 0);
        }
    }


    public void AddForce(Vector3 force)
    {
        velocity += force;
    }


    public void CalculateVelocity()
    {

        // Affect vertical momentum with gravity.
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        // Apply vertical momentum (falling/jumping).
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if (velocity.y < 0)
            velocity.y = checkDownSpeed(transform.position, velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(transform.position, velocity.y);

        isAbleToUp = (checkUpSpeed(transform.position, 1f) > 0);
        isAbleToDown = (checkDownSpeed(transform.position, -1f) < 0);
        isAbleToLeft = left(transform.position);
        isAbleToRight = right(transform.position);
        isAbleToFront = front(transform.position);
        isAbleToBack = back(transform.position);

        isGround = (velocity.y == 0 ? true : false);


        if ((velocity.z > 0 && !isAbleToFront) || (velocity.z < 0 && !isAbleToBack))
            velocity.z = 0;
        if ((velocity.x > 0 && !isAbleToRight) || (velocity.x < 0 && !isAbleToLeft))
            velocity.x = 0;
    }



    // 六個方向碰撞檢查
    public float checkDownSpeed(Vector3 position, float downSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3s(position.x - width / 2f, position.y + downSpeed, position.z - depth / 2f)) ||
            world.CheckForVoxel(new Vector3s(position.x + width / 2f, position.y + downSpeed, position.z - depth / 2f)) ||
            world.CheckForVoxel(new Vector3s(position.x + width / 2f, position.y + downSpeed, position.z + depth / 2f)) ||
            world.CheckForVoxel(new Vector3s(position.x - width / 2f, position.y + downSpeed, position.z + depth / 2f))
           )
            return 0;
        else
            return downSpeed;
    }

    public float checkUpSpeed(Vector3 position, float upSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3s(position.x - width / 2f, position.y + height + upSpeed, position.z - depth / 2f)) ||
            world.CheckForVoxel(new Vector3s(position.x + width / 2f, position.y + height + upSpeed, position.z - depth / 2f)) ||
            world.CheckForVoxel(new Vector3s(position.x + width / 2f, position.y + height + upSpeed, position.z + depth / 2f)) ||
            world.CheckForVoxel(new Vector3s(position.x - width / 2f, position.y + height + upSpeed, position.z + depth / 2f))
           )
            return 0;
        else
            return upSpeed;
    }

    public bool front(Vector3 position)
    {
        if (
            world.CheckForVoxel(new Vector3s(position.x, position.y, position.z + depth / 2f)) ||
            world.CheckForVoxel(new Vector3s(position.x, position.y + 1f, position.z + depth / 2f))
            )
            return false;
        else
            return true;
    }
    public bool back(Vector3 position)
    {
        if (
            world.CheckForVoxel(new Vector3s(position.x, position.y, position.z - depth / 2f)) ||
            world.CheckForVoxel(new Vector3s(position.x, position.y + 1f, position.z - depth / 2f))
            )
            return false;
        else
            return true;
    }
    public bool left(Vector3 position)
    {
        if (
            world.CheckForVoxel(new Vector3s(position.x - width / 2f, position.y, position.z)) ||
            world.CheckForVoxel(new Vector3s(position.x - width / 2f, position.y + 1f, position.z))
            )
            return false;
        else
            return true;
    }
    public bool right(Vector3 position)
    {
        if (
            world.CheckForVoxel(new Vector3s(position.x + width / 2f, position.y, position.z)) ||
            world.CheckForVoxel(new Vector3s(position.x + width / 2f, position.y + 1f, position.z))
            )
            return false;
        else
            return true;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        Vector3 widthPos = new Vector3(width, 0, 0);
        Vector3 depthPos = new Vector3(0, 0, depth);
        Vector3 heightPos = new Vector3(0, height, 0);
        Gizmos.color = Color.green;
        
        // ground
        Gizmos.DrawLine(pos - widthPos / 2 - depthPos / 2, pos + widthPos / 2 - depthPos / 2);
        Gizmos.DrawLine(pos + widthPos / 2 - depthPos / 2, pos + widthPos / 2 + depthPos / 2);
        Gizmos.DrawLine(pos + widthPos / 2 + depthPos / 2, pos - widthPos / 2 + depthPos / 2);
        Gizmos.DrawLine(pos - widthPos / 2 + depthPos / 2, pos - widthPos / 2 - depthPos / 2);

        // side
        Gizmos.DrawLine(pos - widthPos / 2 - depthPos / 2, pos - widthPos / 2 - depthPos / 2 + heightPos);
        Gizmos.DrawLine(pos - widthPos / 2 + depthPos / 2, pos - widthPos / 2 + depthPos / 2 + heightPos);
        Gizmos.DrawLine(pos + widthPos / 2 - depthPos / 2, pos + widthPos / 2 - depthPos / 2 + heightPos);
        Gizmos.DrawLine(pos + widthPos / 2 + depthPos / 2, pos + widthPos / 2 + depthPos / 2 + heightPos);

        // top
        Gizmos.DrawLine(pos - widthPos / 2 - depthPos / 2 + heightPos, pos + widthPos / 2 - depthPos / 2 + heightPos);
        Gizmos.DrawLine(pos + widthPos / 2 - depthPos / 2 + heightPos, pos + widthPos / 2 + depthPos / 2 + heightPos);
        Gizmos.DrawLine(pos + widthPos / 2 + depthPos / 2 + heightPos, pos - widthPos / 2 + depthPos / 2 + heightPos);
        Gizmos.DrawLine(pos - widthPos / 2 + depthPos / 2 + heightPos, pos - widthPos / 2 - depthPos / 2 + heightPos);
    }
}
