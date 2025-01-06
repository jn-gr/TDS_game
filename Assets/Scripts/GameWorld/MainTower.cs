using UnityEngine;

public class MainTower : MonoBehaviour
{
    public float currentHealth;
    //public GameManager gameManager;

    // Function to take away health from currentHealth
    public void TakeDamage(float incomingDamage)
    {
        currentHealth -= incomingDamage;
        GameManager.Instance.UpdateHealth();

 
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    // Function to trigger the TakeDamage function on collision of prefabs
    public void OnTriggerEnter(Collider other)
    {
        // Debug to check what is entering the trigger
        Debug.Log($"Collision detected with: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Enemy"))
        {
            NeutralEnemy enemyComponent = other.GetComponent<NeutralEnemy>();

            if (enemyComponent != null) // Check if the object has the NeutralEnemy component
            {
                if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
                {
                    SoundManager.PlaySound(SoundType.Hurt, 0.5f);

                }
                TakeDamage(enemyComponent.damage);
                enemyComponent.TakeDamage(enemyComponent.health * 100.0f,enemyComponent.element); // Effectively "kills" the enemy

            }
            else
            {
                Debug.LogError($"The object {other.gameObject.name} is tagged as 'Enemy' but does not have a NeutralEnemy component.");
            }
        }
        else
        {
            Debug.Log($"The object {other.gameObject.name} does not have the 'Enemy' tag.");
        }
    }

    
}
