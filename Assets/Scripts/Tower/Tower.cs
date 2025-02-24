using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Tower : MonoBehaviour
{

    public int currentTier;
    protected virtual string towerType => "Turret";
    public float damage;
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
    public CellT cellPlacedOn;

    // Start is called before the first frame update
    public void Start()
    {
        //if (currentTier == 0 && element == Element.Neutral) // spawned in turrets are not placed, but all upgraded either tier or element remain placed
        //{
        //    placed = false;
        //}
        gameManager = GameManager.Instance;
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        var firerateskill = SkillTree.Instance.GetSkill<FireRateSkill>();
        float skillmultiplier = firerateskill.getEffect();
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
                    fireCooldown = 1f / (fireRate * (1 + skillmultiplier));
                }
                fireCooldown -= Time.deltaTime;


            }
            else
            {
                enemyTarget = null;
            }
        }
    }

    public virtual void DecideEnemy()
    {
        Debug.Log("Child has to implement deicde enemy funcition");
        return;
    }

    public virtual void Shoot()
    {      
        var damageBoost = SkillTree.Instance.GetSkill<TowerDamageSkill>();
        Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation, transform);
        projectile.SetForce(projectileForce);
        projectile.SetDamage(damage * damageBoost.getEffect());
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

    public virtual Tower UpgradeTower()
    {
        // 0 is tier 1, 1 is tier 2 you can only upgrade 2 times
        // Check upgrade conditions based on tier and currency
        if ((currentTier == 0 && gameManager.currency >= 100) ||
            (currentTier == 1 && gameManager.currency >= 200))
        {
            // Deduct the appropriate currency
            if (currentTier == 0)
            {
                gameManager.currency -= 100;
            }
            else if (currentTier == 1)
            {
                gameManager.currency -= 200;
            }
            if (currentTier < 2)
            {
                if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
                {
                    SoundManager.PlaySound(SoundType.TowerLevel, 0.5f);

                }
                // the tower type variable is overrided in the child classes, thats how we are able to spawn the correct type of prefab
                Tower upgradedTower = Instantiate(gameManager.GetTowerPrefab(towerType, currentTier + 1, element), transform.position, transform.rotation).GetComponent<Tower>();
                upgradedTower.currentTier = currentTier + 1;
                upgradedTower.element = element;
                upgradedTower.placed = true;
                upgradedTower.cellPlacedOn = cellPlacedOn;
                cellPlacedOn.objectPlacedOnCell = upgradedTower.gameObject;
                Destroy(gameObject);
                return upgradedTower;
            }
            
        }
        return this;
    }
    public virtual Tower FireUpgrade()  
    {
        // need ti implement costs of changing element
        if(element != Element.Fire)
        {
            if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
            {
                SoundManager.PlaySound(SoundType.FireMonsterSound, 0.3f);
            }
            Tower upgradedTower = Instantiate(gameManager.GetTowerPrefab(towerType, currentTier, Element.Fire), transform.position, transform.rotation).GetComponent<Tower>();
            upgradedTower.currentTier = currentTier;
            upgradedTower.element = Element.Fire;
            upgradedTower.placed = true;
            upgradedTower.cellPlacedOn = cellPlacedOn;
            cellPlacedOn.objectPlacedOnCell = upgradedTower.gameObject;
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
            if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
            {
                SoundManager.PlaySound(SoundType.WaterMonsterSound, 0.3f);
            }
            Tower upgradedTower = Instantiate(gameManager.GetTowerPrefab(towerType, currentTier, Element.Water), transform.position, transform.rotation).GetComponent<Tower>();
            upgradedTower.currentTier = currentTier;
            upgradedTower.element = Element.Water;
            upgradedTower.placed = true;
            upgradedTower.cellPlacedOn = cellPlacedOn;
            cellPlacedOn.objectPlacedOnCell = upgradedTower.gameObject;
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
            if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
            {
                SoundManager.PlaySound(SoundType.WindMonsterSound, 0.3f);
            }
            Tower upgradedTower = Instantiate(gameManager.GetTowerPrefab(towerType, currentTier, Element.Air), transform.position, transform.rotation).GetComponent<Tower>();
            upgradedTower.currentTier = currentTier;
            upgradedTower.element = Element.Air;
            upgradedTower.placed = true;
            upgradedTower.cellPlacedOn = cellPlacedOn;
            cellPlacedOn.objectPlacedOnCell = upgradedTower.gameObject;
            Destroy(gameObject);
            return upgradedTower;
        }
        else
        {
            Debug.Log("already this element");
            return this;
        }
        
    }
    public virtual void SellTower()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.3f);
        }
        Destroy(gameObject);
        gameManager.currency += 75;
    }
}
