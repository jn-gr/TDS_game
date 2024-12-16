using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sniper : Tower {

    protected override string towerType => "Sniper";
    public override void DecideEnemy()
    {
        enemiesInRange = enemiesInRange.OrderBy(enemy => enemy.GetComponent<NeutralEnemy>().health).ToList();
        enemyTarget = enemiesInRange[0]; 
    }   
}
