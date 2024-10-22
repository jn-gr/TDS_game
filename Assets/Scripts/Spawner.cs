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

    // couroutine called to start spawning enemies for a wave
    IEnumerator SpawnEnemies()
    {
        while (gameManager.waveStarted && gameManager.CanSpawnEnemy())
        {
            yield return new WaitForSeconds(spawnInterval);

            if (gameManager.CanSpawnEnemy())
            {
                Instantiate(enemyPrefab, transform.position + new Vector3(0,2,0) , Quaternion.identity);
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
