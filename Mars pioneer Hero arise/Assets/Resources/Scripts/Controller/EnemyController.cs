using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* Controls the Enemy AI */

public class EnemyController : MonoBehaviour
{

    public float lookRadius = 10f;  // Detection range for player

    Transform target;   // Reference to the player
    NavAgent agent; // Reference to the NavMeshAgent
    CharacterCombat combat;
    CharacterStats stats;

    // Use this for initialization
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavAgent>();
        combat = GetComponent<CharacterCombat>();
        stats = GetComponent<CharacterStats>();
    }

    // Update is called once per frame
    void Update()
    {
        // Distance to the target
        float distance = Vector3.Distance(target.position, transform.position);

        // If inside the lookRadius
        if (distance <= lookRadius)
        {
            // Move towards the target
            agent.SetDestination(target.position);

            // If within attacking distance
            if (distance <= agent.stoppingDistance)
            {
                CharacterStats targetStats = target.GetComponent<CharacterStats>();
                if (targetStats != null)
                {
                    combat.Attack(targetStats);
                    agent.collider.anim.SetBool("attack", true);
                }
                else
                    agent.collider.anim.SetBool("attack", false);

            }
            FaceTarget();   // Make sure to face towards the target
        }
        else
            agent.SetDestination(agent.transform.position);

        if (stats.currentHealth <= 0)
            StartCoroutine(Die());
    }

    // Rotate to face the target
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction.x !=0 || direction.z != 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)), Time.deltaTime * 5f);
    }

    // Show the lookRadius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    IEnumerator Die()
    {
        foreach (var child in transform)
            if (GetComponent<Renderer>() != null)
                GetComponent<Renderer>().material.color = Color.red;
        for (int i = 0; i < (int)Random.Range(1, 5); i++)
            GameObject.Find("World").GetComponent<World>().DropItem((BlockType)Random.Range(2, 20), transform.position);
        yield return new WaitForSeconds(0.05f);
        Destroy(transform.gameObject);
    }
}