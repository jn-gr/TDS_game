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

    public List<GameObject> enemiesInRange = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyTarget != null)
        {
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }
            fireCooldown -= Time.deltaTime;
        }
    }

    void Shoot()
    {
        Projectile projectile = Instantiate(projectilePrefab,shootPoint.position,shootPoint.rotation); // spawns a projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Debug.Log(enemyTarget.transform.position);
        Vector3 direction = (enemyTarget.transform.position - shootPoint.position).normalized;
        rb.AddForce(direction * projectileForce, ForceMode.Impulse); // using the physics, pushes the projectile in a direction
        projectile.SetForce(projectileForce);
        projectile.SetDamage(damage);
        projectile.SetTarget(enemyTarget);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered with: " + other.name);
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
