using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : Tower
{
    public int maxSimultaneousTargets;
    private List<GameObject> enemyTargets = new List<GameObject>();


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
                Debug.Log("shoot");
                Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation, transform); // spawns a projectile
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                Vector3 direction = (target.transform.position - shootPoint.position).normalized;
                rb.AddForce(direction * projectileForce, ForceMode.Impulse); // using the physics, pushes the projectile in a direction
                projectile.SetForce(projectileForce);
                projectile.SetDamage(damage);
                projectile.SetTarget(target);
                projectile.SetElement(element);
            }
        }
        
    }

}
