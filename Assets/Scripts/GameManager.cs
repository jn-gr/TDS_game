using System.Collections;
using System.Collections.Generic;
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
    public MainTower mainTower;
    public Spawner[] spawners;

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

    void Start()
    {
        spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
        currentHealth = mainTower.GetHealth();
        currency = 1000;
        waveStarted = false;

    }

    void Update()
    {
        if (waveStarted && enemiesSpawned == totalEnemiesToSpawn && enemiesAlive == 0)
        {
            EndWave();
        }
    }

    public void UpdateHealth()
    {
        currentHealth = mainTower.GetHealth();
    }

    public void EnemyKilled()
    {
        totalKills++;
        currency += 50;
        score += 100;
        experience += 100;
        enemiesAlive--; ;

        Debug.Log($"Wave {waveNum}: Enemy {enemiesAlive} has been killed");
    }


    public void StartWave()
    {
        waveStarted = true;
        waveNum++;
        totalEnemiesToSpawn = 4 + Mathf.RoundToInt(waveNum * 1.2f);
        enemiesSpawned = 0;
        enemiesAlive = totalEnemiesToSpawn;

        foreach (Spawner spawner in spawners)
        {
            spawner.StartSpawning();
        }

        Debug.Log($"Wave {waveNum} started: Spawning {totalEnemiesToSpawn} enemies.");
    }

    public void EndWave()
    {
        waveStarted = false;
        

        foreach (Spawner spawner in spawners)
        {
            spawner.StopSpawning();
        }
        Debug.Log($"Wave {waveNum} ended: Total Kills = {totalKills}, Score = {score}, Currency = {currency}, Experience = {experience}.");
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

