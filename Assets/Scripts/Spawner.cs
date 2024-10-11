using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    public GameObject enemyPrefab;  // The enemy prefab to spawn
        // The point where enemies will spawn
    public float spawnInterval = 1.0f;  // Time interval between spawns (1 second)

    // Start is called before the first frame update
    void Start()
    {
        // Start the spawning coroutine
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Spawn the enemy
            Instantiate(enemyPrefab, gameObject.transform.position, gameObject.transform.rotation);
            // Wait for the specified interval before spawning the next enemy
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
