public class AirEnemy : NeutralEnemy
{
    public override void Start()
    {
        base.Start();
        element = Element.Air;
    }
    public override void TakeDamage(float damage, Element element)
    {
        if (isDead) return;

        // Same type element damage has a damage multiplier of 0.9
        if (element == Element.Air)
        {
            health -= damage * selfDamageMultiplier;
        }
        // Stronger element damage has a damage multiplier of 1.2
        else if (element == Element.Fire)
        {
            health -= damage * strongDamageMultiplier;
        }
        // Weaker element damage has a damage multiplier of 0.8
        else if (element == Element.Water)
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
