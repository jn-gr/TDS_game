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
                // Find a walkable tile near the spawner position
                Vector3Int cellPosition = MapManager.Instance.tilemap.WorldToCell(transform.position);
                Vector3Int walkableTile = MapManager.Instance.FindNearestWalkableTile(cellPosition);

                if (walkableTile != null)
                {
                    Vector3 spawnPosition = MapManager.Instance.tilemap.GetCellCenterWorld(walkableTile);
                    // Adjust the Y position to ensure mobs are not clipping
                    spawnPosition.y += 0.4f;

                    // Adjust rotation to face the main tower
                    Vector3 directionToTower = (mainTower.position - spawnPosition).normalized;
                    Quaternion spawnRotation = Quaternion.LookRotation(directionToTower);


                    // Instantiate the enemy with position and rotation
                    SoundManager.PlaySound(SoundType.MonsterSpawn, 0.1f);
                    Instantiate(enemyPrefab, spawnPosition, spawnRotation);

                    gameManager.EnemySpawned();
                }
            }
            else
            {
                break;
            }
        }
        spawnCoroutine = null;
    }
}
