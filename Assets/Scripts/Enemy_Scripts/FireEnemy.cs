using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

public class Fire_Enemy : MonoBehaviour, IDamageable
{
    public int damage;
    public int health;
    public float speed;
    private MainTower castle;
    private GameManager gameManager;
    private bool isDead;
    public Element element;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        castle = gameManager.mainTower;
        health = (int)(8 + (gameManager.waveNum * 1.1));
        speed = (float)(5 + (gameManager.waveNum * 1.5));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, castle.transform.position, speed * Time.deltaTime);
    }

    public void TakeDamage(int damage, Element element)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            gameManager.EnemyKilled();
            Destroy(gameObject);
        }
    }
}
