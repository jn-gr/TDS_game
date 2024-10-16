using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MainTower mainTower;
    public int currentHealth;
    public int currency;
    public int experience;
    public int totalKills;
    public int score;

    public bool waveStarted;
    public int enemiesInThisWave;
    public int enemiesLeftInWave; // Used internally to tell spawners how many left to spawn
    public int enemiesDiedThisWave;


    //Setting defaults on game start
    void Start()
    {
        currentHealth = mainTower.GetHealth();
        currency = 1000;
        waveStarted = false;
    }

    void Update()
    {
        // This runs if wave started and you've killed all the enemies.
        // I say enemies died to account for if any died by killing themselves when they hit the castle.

        if (waveStarted && enemiesLeftInWave == 0 && enemiesDiedThisWave == enemiesInThisWave) 
        {
            waveStarted = false;
            enemiesDiedThisWave = 0;
        }
    }

    public void UpdateHealth()
    {
        currentHealth = mainTower.GetHealth();
    }
    public void EnemyKilled()
    {
        totalKills += 1;
        currency += 50;
        score += 100;
        experience += 100;

        enemiesDiedThisWave += 1;
    }

    public void NewEnemiesSpawned(int numberOfNewEnemiesSpawned)
    {
        enemiesLeftInWave -= numberOfNewEnemiesSpawned;
    }

    // Starts the first wave. Could be generalised for multiple waves. It isn't right now, though.
    public void StartWaveOne()
    {
        waveStarted = true;
        enemiesInThisWave = 19;
        enemiesLeftInWave = enemiesInThisWave;
    }
}
