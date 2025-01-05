using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


public enum Element
{
    Neutral = 0,
    Fire = 1,
    Water = 2,
    Air = 3
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public MainTower mainTower;
    //public Spawner[] spawners;

    public float currentHealth;
    public int currency;
    public int experience;
    public int totalKills;
    public int score;
    public int waveNum;
    public bool waveStarted;

    public int totalEnemiesToSpawn;
    public int enemiesSpawned;
    public int enemiesAlive;


    [Header("Tower Prefab Pool")]

    // lvl1 nuetral,fire,water,air, lv2 so on
    public GameObject[] turretPrefabs; // 12 prefabs 
    public GameObject[] sniperPrefabs; // 12 prefabs 
    public GameObject[] rapidFirePrefabs; // 12 prefabs 

    public GameObject[] tierOneEnemy; // Neutral, Fire, Water, Air Tier One Enemies
    public GameObject[] tierTwoEnemy; // Neutral, Fire, Water, Air Tier Two Enemies
    public GameObject[] boss; // Neutral, Fire, Water, Air Boss Enemies

    public event Action WaveEnded;
    public event Action WaveStarted;
    public event Action LastWaveCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("Level chosen is : " + UserDifficulty.CurrentLevel);


        currentHealth = mainTower.GetHealth();
        currency = 1000;
        waveStarted = false;
    }

    // Helper method to log array details
    void LogArray(string arrayName, GameObject[] array)
    {
        Debug.Log($"{arrayName} Size: {array.Length}");
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log($"{arrayName}[{i}]: {(array[i] != null ? array[i].name : "NULL")}");
        }
    }

    void Update()
    {
        if (waveStarted && enemiesSpawned == totalEnemiesToSpawn && enemiesAlive == 0)
        {
            WaveEnded?.Invoke();
            EndWave();
            GetComponent<UI>().EndOfWave();
        }
    }

    public void UpdateHealth()
    {
        currentHealth = Mathf.Round(mainTower.GetHealth());
    }

    public void EnemyKilled()
    {
        totalKills++;
        currency += 50;
        score += 100;
        experience += 100;
        enemiesAlive--;

        //skill tree multipliers
        experience *= XpBoostSkill.getEffect();
        currency *= GoldEarnSkill.getEffect();

        Debug.Log($"Wave {waveNum}: Enemy killed. Enemies alive: {enemiesAlive}");
    }


    public void StartWave()
    {
        waveStarted = true;
        waveNum++;

        // Scale stats for all NeutralEnemies based on the current wave number
        NeutralEnemy.ScaleStatsForWave(waveNum);

        totalEnemiesToSpawn = 4 + Mathf.RoundToInt(waveNum * 1.2f);
        enemiesSpawned = 0;
        enemiesAlive = totalEnemiesToSpawn;

        // Assign random prefabs to spawners and start spawning
        foreach (KeyValuePair<(int x, int y), Spawner> spawner in MapManager.Instance.spawnerPositions)
        {
            if (waveNum >= 10 && waveNum % 10 == 0)
            {
                int randomBossIndex = UnityEngine.Random.Range(0, boss.Length); // Corrected range
                spawner.Value.enemyPrefab = boss[randomBossIndex];
            }
            else
            {
                int randomTier = UnityEngine.Random.Range(0, 2); // Choose between Tier One and Tier Two
                if (randomTier == 0)
                {
                    int randomEnemyIndex = UnityEngine.Random.Range(0, tierOneEnemy.Length); // Corrected range
                    spawner.Value.enemyPrefab = tierOneEnemy[randomEnemyIndex];
                }
                else
                {
                    int randomEnemyIndex = UnityEngine.Random.Range(0, tierTwoEnemy.Length); // Corrected range
                    spawner.Value.enemyPrefab = tierTwoEnemy[randomEnemyIndex];
                }
            }

            spawner.Value.StartSpawning();
        }

        Debug.Log($"Wave {waveNum} started: Spawning {totalEnemiesToSpawn} enemies.");
        WaveStarted?.Invoke();
    }

    public int GetLastWaveNumber()
    {
        DifficultyLevel difficulty = UserDifficulty.CurrentLevel;

        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                return 50;
            case DifficultyLevel.Medium:
                return 100;
            case DifficultyLevel.Hard:
                return 150;
            default:
                return 1; // Should only show up in development.
        }
    }

    public void EndWave()
    {
        waveStarted = false;


        foreach (KeyValuePair<(int x, int y), Spawner> spawner in MapManager.Instance.spawnerPositions)
        {
            spawner.Value.StopSpawning();

        }
        Debug.Log($"Wave {waveNum} ended: Total Kills = {totalKills}, Score = {score}, Currency = {currency}, Experience = {experience}.");

        if (waveNum == GetLastWaveNumber())
        {
            LastWaveCompleted?.Invoke();
        }
    }

    public void EnemySpawned()
    {
        enemiesSpawned++;
        Debug.Log($"Wave {waveNum}: Enemy {enemiesSpawned} has been spawned");

    }

    public bool CanSpawnEnemy()
    {
        return enemiesSpawned < totalEnemiesToSpawn;
    }

    public GameObject GetTowerPrefab(string towerType, int tier, Element element)
    {

        int prefabIndex = tier * 4 + (int)element; // Calculate index in 1D array


        if (towerType == "Turret")
        {
            return turretPrefabs[prefabIndex];
        }
        else if (towerType == "Sniper")
        {
            return sniperPrefabs[prefabIndex];
        }
        else if (towerType == "RapidFire")
        {
            return rapidFirePrefabs[prefabIndex];
        }
        else
        {
            Debug.LogError("tower tpye error");
            return null;
        }
    }

}
