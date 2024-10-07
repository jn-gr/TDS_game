using UnityEngine;

public class MainTower: MonoBehaviour
{
    //Main variables
    public int maxHealth = 100;
    public int damageTaken = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

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

    //Function to trigger the TakeDamage function on collision of prefabs
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    //Function when currentHealth is <= 0
    private void EndGame()
    {
        Debug.Log("Main Tower destroyed");
    }
}