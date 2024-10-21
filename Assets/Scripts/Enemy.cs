using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int damage;
    public int health;
    public float speed;
    private MainTower castle;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>(); 
        castle = gameManager.mainTower;
        health = (int)(8 + (gameManager.waveNum * 1.1));
        speed = (float)(2 + (gameManager.waveNum * 1.5));

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, castle.transform.position, speed * Time.deltaTime);
    }
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            gameManager.EnemyKilled();
            Destroy(gameObject);
        }
    }
}
