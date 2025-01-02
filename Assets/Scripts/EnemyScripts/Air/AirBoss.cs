using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBoss : AirEnemy
{
    public override void Start()
    {
        base.Start();

        // Multiply damage, health and speed for boss
        damage *= 5;
        health *= 6;
        speed *= 0.8f;

    }
}
