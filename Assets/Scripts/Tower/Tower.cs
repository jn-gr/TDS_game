using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Tower : MonoBehaviour
{

    public int currentTier;
    protected virtual string towerType => "Turret";
    public int damage;
    public Element element;
    
    public Projectile projectilePrefab;
    public Transform shootPoint;
    public float projectileForce;
    public float fireRate;

    [SerializeField]protected float fireCooldown;

    protected GameManager gameManager;

    [Header("Debugging")]
    public GameObject enemyTarget;

    [SerializeField]  protected List<GameObject> enemiesInRange = new List<GameObject>();

    public bool placed;

    // Start is called before the first frame update
    public void Start()
    {
        placed = false;
        gameManager = FindAnyObjectByType<GameManager>();
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (placed)
        {
            // Clean up any destroyed enemies from the list
            enemiesInRange.RemoveAll(e => e == null);
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

    public virtual Tower UpgradeTower() {
        // 0 is tier 1, 1 is tier 2 you can only upgrade 2 times
        if (currentTier < 2)
        {
            // the tower type variable is overrided in the child classes, thats how we are able to spawn the correct type of prefab
            Tower upgradedTower = Instantiate(gameManager.GetTowerPrefab(towerType, currentTier + 1, element), transform.position, transform.rotation).GetComponent<Tower>();
            upgradedTower.currentTier = currentTier + 1;
            upgradedTower.element = element;
            Destroy(gameObject);
            return upgradedTower;
        }
        return this;
    } 
    public virtual Tower FireUpgrade()  
    {
        // need ti implement costs of changing element
        if(element != Element.Fire)
        {
            Tower upgradedTower = Instantiate(gameManager.GetTowerPrefab(towerType, currentTier, Element.Fire), transform.position, transform.rotation).GetComponent<Tower>();
            
            upgradedTower.element = Element.Fire;
            Destroy(gameObject);
            return upgradedTower;
        }
        else
        {
            Debug.Log("already this element");
            return this;
        }
    }
    public virtual Tower WaterUpgrade() 
    {
        if (element != Element.Water)
        {
            Tower upgradedTower = Instantiate(gameManager.GetTowerPrefab(towerType, currentTier, Element.Water), transform.position, transform.rotation).GetComponent<Tower>();
            
            upgradedTower.element = Element.Water;
            Destroy(gameObject);
            return upgradedTower;
        }
        else
        {
            Debug.Log("already this element");
            return this;
        }
    }
    public virtual Tower AirUpgrade() 
    {
        if (element != Element.Air)
        {
            Tower upgradedTower = Instantiate(gameManager.GetTowerPrefab(towerType, currentTier, Element.Air), transform.position, transform.rotation).GetComponent<Tower>();
            
            upgradedTower.element = Element.Air;
            Destroy(gameObject);
            return upgradedTower;
        }
        else
        {
            Debug.Log("already this element");
            return this;
        }
        
    }

}
