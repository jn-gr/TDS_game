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

        if (currentHealth <= 0)
        {
            EndGame();
        }
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

        if (other.CompareTag("Enemy")) // Ensure the tag matches exactly with Unity's tag system
        {
            NeutralEnemy enemyComponent = other.GetComponent<NeutralEnemy>();

            if (enemyComponent != null) // Check if the object has the NeutralEnemy component
            {
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

    // Function when currentHealth is <= 0
    private void EndGame()
    {
        Debug.Log("Main Tower destroyed");
        // Add logic here to handle the end game state (e.g., show game over UI, restart the level, etc.)
    }
}
