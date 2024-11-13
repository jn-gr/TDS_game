using UnityEngine;

public class FireEnemy : NeutralEnemy
{
    public float levitationHeight = 5f; // Max height to levitate
    public float levitationSpeed = 5f;   // Speed of levitation
    private float startingY;              // Starting Y position

    public override void Start()
    {
        base.Start();
        element = Element.Fire;
        startingY = transform.position.y;  // Set starting Y to initial Y
    }

    public override void Update()
    {
        base.Update();

        // Calculate smooth levitation effect with sine wave
        float currentHeight = Mathf.Sin(Time.time * levitationSpeed) * levitationHeight;

        // Apply the levitation effect to the Y position
        transform.position = new Vector3(transform.position.x, startingY + currentHeight, transform.position.z);
    }

    public override void TakeDamage(float damage, Element element)
    {
        if (isDead) return;

        // Same type element damage has a damage multiplier of 0.9
        if (element == Element.Fire)
        {
            health -= damage * selfDamageMultiplier;
        }
        // Stronger element damage has a damage multiplier of 1.2
        else if (element == Element.Water)
        {
            health -= damage * strongDamageMultiplier;
        }
        // Weaker element damage has a damage multiplier of 0.8
        else if (element == Element.Air)
        {
            health -= damage * weakDamageMultiplier;
        }
        // Neutral element damage always does base damage
        else
        {
            health -= damage;
        }

        if (health <= 0)
        {
            isDead = true;
            gameManager.EnemyKilled();
            Destroy(gameObject);
        }
    }
}
