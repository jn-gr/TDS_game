using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralEnemyTier2 : NeutralEnemy
{
    public override void Start()
    {
        base.Start();
        Tier = 2;
        // Multiply damage, health and speed for Tier 2
        damage *= 2;
        health *= 1.5f;
        speed *= 1.2f;

    }
}
