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

    //Setting defaults on game start
    void Start()
    {
        currentHealth = mainTower.GetHealth();
        currency = 1000;
    }

    void Update()
    {
    }

    public void updateHealth()
    {
        currentHealth = mainTower.GetHealth();
    }
    public void enemyKilled()
    {
        totalKills += 1;
        currency += 50;
        score += 100;
        experience += 100;
    }
}
