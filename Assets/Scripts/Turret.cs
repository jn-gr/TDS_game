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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
        fireCooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        Projectile projectile = Instantiate(projectilePrefab,shootPoint.position,shootPoint.rotation); // spawns a projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(shootPoint.right * projectileForce, ForceMode.Impulse); // using the physics, pushes the projectile in a direction

        projectile.SetDamage(damage);
    }
    
}
