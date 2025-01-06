using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : Tower
{
   
    public int maxSimultaneousTargets;
    private List<GameObject> enemyTargets = new List<GameObject>();

    protected override string towerType => "RapidFire";

    public override void Update()
    {
        if (placed)
        {
            enemiesInRange.RemoveAll(e => e == null);
            if (enemiesInRange.Count >0)
            {
                DecideEnemy();
                if (fireCooldown <= 0f)
                {
                    Shoot();
                    fireCooldown = 1f / fireRate;
                }
                fireCooldown -= Time.deltaTime;


            }
            else
            {
                enemyTargets.Clear();
            }
        }
    }
    public override void DecideEnemy()
    {

       
        // Clear previous targets
        enemyTargets.Clear();

        // Add up to 3 enemies from the range list
        for (int i = 0; i < Mathf.Min(maxSimultaneousTargets, enemiesInRange.Count); i++)
        {
            enemyTargets.Add(enemiesInRange[i]);
        }
        

    }
    public override void Shoot()
    {
        
        foreach (GameObject target in enemyTargets)
        {
            if (target != null)
            {
                var damageBoost = SkillTree.Instance.GetSkill<TowerDamageSkill>();
                Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation, transform);
                projectile.SetForce(projectileForce);
                projectile.SetDamage(damage * damageBoost.getEffect());
                projectile.SetTarget(enemyTarget);
                projectile.SetElement(element);
            }
        } 
    }
}
