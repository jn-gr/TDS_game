using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject enemyPrefab;  // The enemy prefab to spawn
                                    // The point where enemies will spawn
    public float spawnInterval;  // Time interval between spawns (1 second)

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Start the spawning coroutine
        StartCoroutine(SpawnEnemies());

        gameManager = FindFirstObjectByType<GameManager>();
    }

    // This just runs this code every spawn interval. Then only runs it if the wave has started.
    // Should only run when wave is started and not before.
    // Fix later.

    // Another issue is that spawners spawn at the same time. If it's a case where only 1 of them needs to spawn, what might happen is both spawn. Then you get 1 extra enemy.
    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Wait for the specified interval before spawning the next enemy
            yield return new WaitForSeconds(spawnInterval);

            // Checks game manager if theres enemies left to spawn.
            if (gameManager.waveStarted && gameManager.enemiesLeftInWave > 0)
            {
                // Spawn the enemy
                Instantiate(enemyPrefab, gameObject.transform.position, gameObject.transform.rotation);
                // Tell game manager we spawned a new enemy I'm telling it we just spawned 1.
                gameManager.NewEnemiesSpawned(1);
            }
        }


    }
}