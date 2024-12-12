using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sniper : Tower { 
    
   public override void DecideEnemy()
   {
        

        if (enemiesInRange.Count > 0)
        {
            
            enemiesInRange = enemiesInRange.OrderBy(enemy => enemy.GetComponent<NeutralEnemy>().health).ToList();


           

            enemyTarget = enemiesInRange[0]; 
        }
        else
        {
            enemyTarget = null; 
        }

    }
}
