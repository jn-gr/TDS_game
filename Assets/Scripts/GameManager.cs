using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class GameManager : MonoBehaviour
//{
//    public MainTower mainTower;
//    public int currentHealth;
//    public int currency;
//    public int experience;
//    public int totalKills;
//    public int score;
//    public int waveNum;
//    public bool waveStarted;
//    public int enemiesInThisWave;
//    public int enemiesLeftInWave; // Used internally to tell spawners how many left to spawn
//    public int enemiesDiedThisWave;


//    //Setting defaults on game start
//    void Start()
//    {
//        currentHealth = mainTower.GetHealth();
//        currency = 1000;
//        waveStarted = false;
//    }

//    void Update()
//    {
//        // This runs if wave started and you've killed all the enemies.
//        // I say enemies died to account for if any died by killing themselves when they hit the castle.

//        if (waveStarted && enemiesLeftInWave == 0 && enemiesDiedThisWave == enemiesInThisWave) 
//        {
//            waveStarted = false;
//            enemiesDiedThisWave = 0;
//        }
//    }

//    public void UpdateHealth()
//    {
//        currentHealth = mainTower.GetHealth();
//    }
//    public void EnemyKilled()
//    {
//        totalKills += 1;
//        currency += 50;
//        score += 100;
//        experience += 100;

//        enemiesDiedThisWave += 1;
//    }

//    public void NewEnemiesSpawned(int numberOfNewEnemiesSpawned)
//    {
//        enemiesLeftInWave -= numberOfNewEnemiesSpawned;
//    }

//    public void StartWave()
//    {
//        waveStarted = true;
//        enemiesInThisWave = (int)(4 + (waveNum * 0.2));
//        enemiesLeftInWave = enemiesInThisWave;
//        waveNum++;
        

//    }
//}

public class GameManager : MonoBehaviour
{
    public MainTower mainTower;
    public Spawner[] spawners;

    public int currentHealth;
    public int currency;
    public int experience;
    public int totalKills;
    public int score;
    public int waveNum;
    public bool waveStarted;


    //public int enemiesInThisWave;
    //public int enemiesLeftInWave;
    //public int enemiesDiedThisWave;

    public int totalEnemiesToSpawn;
    public int enemiesSpawned;
    public int enemiesAlive;

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

}

