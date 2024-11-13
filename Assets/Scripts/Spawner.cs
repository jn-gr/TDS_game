using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval;
    private GameManager gameManager;
    private Coroutine spawnCoroutine;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void StartSpawning()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    // Coroutine called to start spawning enemies for a wave
    IEnumerator SpawnEnemies()
    {
        while (gameManager.waveStarted && gameManager.CanSpawnEnemy())
        {
            yield return new WaitForSeconds(spawnInterval);

            if (gameManager.CanSpawnEnemy())
            {
                // Determine rotation based on the enemy type
                Quaternion spawnRotation = Quaternion.identity;
                Vector3 spawnPosition = transform.position + new Vector3(0, 2, 0); // Base spawn position with Y offset

                // Check if the enemy is a FireEnemy and increase Y position by 1
                if (enemyPrefab.GetComponent<FireEnemy>() != null)
                {
                    spawnRotation = Quaternion.Euler(-90, 0, 0);
                    spawnPosition.y += 1f;  // Increase Y position by 1 for FireEnemy
                }

                // Instantiate with position and the specified rotation
                Instantiate(enemyPrefab, spawnPosition, spawnRotation);
                gameManager.EnemySpawned();
            }
            else
            {
                break;
            }
        }
        spawnCoroutine = null;
    }
}
