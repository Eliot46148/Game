using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneration : MonoBehaviour
{
    public List<GameObject> enemyPrefab = new List<GameObject>();
    public float spawnRadius = 10f;

    Transform target;   // Reference to the player
    World world;


    public float spawnSpeed = 1f;
    private float spawnCooldown = 0f;

    public float spawnDelay = .6f;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        world = GameObject.Find("World").GetComponent<World>();
    }

    void Update()
    {
        if (!world.isCreative)
        {
            spawnCooldown -= Time.deltaTime;
            if (Random.Range(0, 100) < 10)
                Spawn();
        }
    }

    public void Spawn()
    {
        if (spawnCooldown <= 0f)
        {
            StartCoroutine(DoSpawn(spawnDelay));

            spawnCooldown = 1f / spawnSpeed;
        }

    }

    IEnumerator DoSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 position = target.position + new Vector3(Random.Range(-spawnRadius / 2, spawnRadius), 0, Random.Range(-spawnRadius / 2, spawnRadius));
        position = world.GetGroundY(position);

        GameObject enemy = enemyPrefab[(int)Random.Range(0, enemyPrefab.Count)];
        GameObject drop = Instantiate(enemy, position, Quaternion.identity);
        drop.transform.SetParent(GameObject.Find("Enemies").transform);
    }

    // Show the lookRadius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, spawnRadius);
    }
}
