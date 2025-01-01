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
    protected MainTower castle;
    protected GameManager gameManager;
    protected bool isDead;
    public Element element;
    public float levitationHeight = 5f; // Max height to levitate
    public float levitationSpeed = 5f;   // Speed of levitation
    private float startingY;              // Starting Y position

    // Start is called before the first frame update
    public virtual void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        castle = gameManager.mainTower;
        health = (int)(8 + (gameManager.waveNum * 1.1));
        speed = (float)(5 + (gameManager.waveNum * 1.5));
        element = Element.Neutral;
        startingY = transform.position.y;  // Set starting Y to initial Y
    }

    // Update is called once per frame
    public virtual void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, castle.transform.position, speed * Time.deltaTime);

        // Calculate smooth levitation effect with sine wave (shifted to stay above the plane)
        float currentHeight = Mathf.Abs(Mathf.Sin(Time.time * levitationSpeed)) * levitationHeight;

        // Apply the levitation effect to the Y position
        transform.position = new Vector3(transform.position.x, startingY + currentHeight, transform.position.z);
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
