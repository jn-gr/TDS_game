using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    public int damage;
    public Element element;
    public Projectile projectilePrefab;
    public Transform shootPoint;
    public float projectileForce;
    public float fireRate;

    protected float fireCooldown;


    [Header("Debugging")]
    public GameObject enemyTarget;
    
    protected List<GameObject> enemiesInRange = new List<GameObject>();

    public bool placed;

    // Start is called before the first frame update
    public void Start()
    {
        placed = false;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (placed)
        {
            // Clean up any destroyed enemies from the list
            
            if (enemiesInRange.Count > 0)
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
                enemyTarget = null;
            }
            //else
            //{
            //    // Clean up any destroyed enemies from the list
            //    enemiesInRange.RemoveAll(e => e == null);

            //    DecideEnemy();
            //}
        }
    }

    public virtual void DecideEnemy()
    {
        Debug.Log("Child has to implement deicde enemy funcition");
        return;
    }

    public virtual void Shoot()
    {
        Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation, transform); // spawns a projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Vector3 direction = (enemyTarget.transform.position - shootPoint.position).normalized;
        rb.AddForce(direction * projectileForce, ForceMode.Impulse); // using the physics, pushes the projectile in a direction
        projectile.SetForce(projectileForce);
        projectile.SetDamage(damage);
        projectile.SetTarget(enemyTarget);
        projectile.SetElement(element);

        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            enemiesInRange.Remove(enemy);  
        }
    }

}
