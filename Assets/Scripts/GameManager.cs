using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MainTower mainTower;
    public int currentHealth;
    private int currency = 0;
    private int experience = 0;
    private int totalKills = 0;
    private int Score = 0;

    //Setting defaults on game start
    void Start()
    {
        mainTower.SetHealth(100);
        currentHealth = mainTower.GetHealth();
    }

    void Update()
    {
        Score = totalKills * 5;
    }
}
