using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public int maxHealth;
    public int damage;
    public Projectile projectilePrefab;
    public Transform shootPoint;
    public float projectileForce;
    public float fireRate;
    public float fireCooldown;

    public GameObject enemyTarget;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    public bool placed;

    // Start is called before the first frame update
    void Start()
    {
        placed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (placed)
        {
            if (enemyTarget != null)
            {
                if (fireCooldown <= 0f)
                {
                    Shoot();
                    fireCooldown = 1f / fireRate;
                }
                fireCooldown -= Time.deltaTime;
               
                if (enemyTarget == null)
                {
                    enemyTarget = null;
                }
            }
            else
            {
                // we remove all enemies that have been destoryed in the list
                enemiesInRange.RemoveAll(e => e == null);

                // if there is an enemy still left within range, assign the first enemy in the list as target
                if (enemiesInRange.Count > 0)
                {
                    enemyTarget = enemiesInRange[0];
                }
            }
        }
    }


    void Shoot()
    {
        Projectile projectile = Instantiate(projectilePrefab,shootPoint.position,shootPoint.rotation,transform); // spawns a projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Vector3 direction = (enemyTarget.transform.position - shootPoint.position).normalized;

        rb.AddForce(direction * projectileForce, ForceMode.Impulse); // using the physics, pushes the projectile in a direction
        projectile.SetForce(projectileForce);
        projectile.SetDamage(damage);
        projectile.SetTarget(enemyTarget);
    }

    private void OnTriggerEnter(Collider other)
    {       
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            enemiesInRange.Add(enemy);

            if (enemyTarget == null)
            {
                enemyTarget = enemy;
            }  
        }
    }

    // we check to see if enemy collider exit range so that we can remove it from the list
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            enemiesInRange.Remove(enemy);

            if (enemy == enemyTarget)
            {
                if (enemiesInRange.Count > 0)
                {
                    
                    enemyTarget = enemiesInRange[0];
                }
                else
                {
                    enemyTarget = null; 
                }
            }
        }
    }

}
