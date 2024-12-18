using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Element
{
    Fire,
    Water,
    Neutral,
    Air
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

    public event Action WaveEnded;

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
            WaveEnded?.Invoke();
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

