using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval;
    private GameManager gameManager;
    private Coroutine spawnCoroutine;

    // Reference to the main tower
    public Transform mainTower;

    void Start()
    {
        gameManager = GameManager.Instance;
        mainTower = gameManager.mainTower.transform;
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
                // Determine spawn position
                Vector3 spawnPosition = transform.position + new Vector3(0, 2, 0); // Base spawn position with Y offset

                // Calculate rotation to face the main tower
                Vector3 directionToTower = (mainTower.position - spawnPosition).normalized;
                Quaternion spawnRotation = Quaternion.LookRotation(directionToTower);

                // Adjust rotation to account for Blender's forward axis
                Quaternion adjustedRotation = spawnRotation * Quaternion.Euler(-90, 180, 0);

                // Check if the enemy is a FireEnemy and adjust Y position
                if (enemyPrefab.GetComponent<FireEnemy>() != null)
                {
                    spawnPosition.y += 1f;  // Increase Y position by 1 for FireEnemy
                }

                // Instantiate the enemy with position and adjusted rotation
                Instantiate(enemyPrefab, spawnPosition, adjustedRotation);
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
