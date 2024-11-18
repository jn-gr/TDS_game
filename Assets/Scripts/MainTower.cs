using UnityEngine;

public class MainTower : MonoBehaviour
{
    public int currentHealth;
    public GameManager gameManager;

    //Function to take away health from currentHealth
    public void TakeDamage(int incomingDamage)
    {
        currentHealth -= incomingDamage;
        gameManager.UpdateHealth();

        if (currentHealth <= 0)
        {
            EndGame();
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    //Function to trigger the TakeDamage function on collision of prefabs
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            TakeDamage(enemy.GetComponent<Enemy>().damage);
            Destroy(other.gameObject);           

            // Tell game manager that an enemy died when they hit the tower. So enemy wave death counts work.
            gameManager.enemiesAlive --;
        }
    }

    //Function when currentHealth is <= 0
    private void EndGame()
    {
        Debug.Log("Main Tower destroyed");
    }

}