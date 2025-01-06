using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Tower
{
    protected override string towerType => "Turret";
    public override void DecideEnemy()
    {
        if (enemiesInRange.Count > 0)
        {
            enemyTarget = enemiesInRange[0];
        }
    }
}