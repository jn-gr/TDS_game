using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int damage;
    public int health;
    public float speed;
    private MainTower castle;
    private GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
       
        manager = FindFirstObjectByType<GameManager>(); 
        castle = manager.mainTower;
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

            Destroy(gameObject);
        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Castle"))
        {
            Debug.Log("castle has been hit by monster");
        }
    }
}
