using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralEnemy : MonoBehaviour
{
    public float damage = 10;
    public float health = 20;
    public float speed = 5;
    public float selfDamageMultiplier = 0.9f;
    public float strongDamageMultiplier = 1.2f;
    public float weakDamageMultiplier = 0.8f;
    private MainTower castle;
    protected GameManager gameManager;
    protected bool isDead;
    public Element element;

    // Start is called before the first frame update
    public virtual void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        castle = gameManager.mainTower;
        health = (int)(8 + (gameManager.waveNum * 1.1));
        speed = (float)(5 + (gameManager.waveNum * 1.5));
        element = Element.Neutral;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, castle.transform.position, speed * Time.deltaTime);
    }

    public virtual void TakeDamage(float damage, Element element)
    {
        if (isDead) return;

        health -= damage * 1.0f;

        if (health <= 0)
        {
            isDead = true;
            gameManager.EnemyKilled();
            Destroy(gameObject);
        }
    }
}
