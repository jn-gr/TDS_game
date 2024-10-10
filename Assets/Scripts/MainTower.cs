using UnityEngine;

public class MainTower : MonoBehaviour
{
    private int maxHealth;
    private int damageTaken;
    private int currentHealth;

    //Function to take away health from currentHealth
    public void TakeDamage()
    {
        currentHealth -= damageTaken;

        //Check if currentHealth is not <=0
        if (currentHealth <= 0)
        {
            EndGame();
        }
    }

    public void SetHealth(int hp)
    {
        maxHealth = hp;
        currentHealth = hp;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void SetDamageTaken(int dmg)
    {
        damageTaken = dmg;
    }

    //Function to trigger the TakeDamage function on collision of prefabs
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;

            TakeDamage();
            Destroy(other.gameObject);

            Debug.Log("i got hit by enemy");
        }
    }

    //Function when currentHealth is <= 0
    private void EndGame()
    {
        Debug.Log("Main Tower destroyed");
    }
}