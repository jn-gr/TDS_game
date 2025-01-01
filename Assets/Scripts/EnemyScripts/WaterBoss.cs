using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBoss : WaterEnemy
{
    public override void Start()
    {
        base.Start();

        // Multiply damage, health and speed for boss
        damage *= 3;
        health *= 10;
        speed *= 0.8f;

    }
}
