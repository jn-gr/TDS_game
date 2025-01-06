using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralBoss : NeutralEnemy
{
    public override void Start()
    {
        base.Start();
        Tier = 3;
        // Multiply damage, health and speed for boss
        damage *= 5;
        health *= 2.5f;
        speed *= 0.8f;

    }
}