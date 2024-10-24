using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float force;
    public GameObject enemyTarget;
    // Start is called before the first frame update
    private Rigidbody rb;
    [SerializeField] private float turnSpeed;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Destroy(gameObject, 5); // dies after 5 seconds
    }

    
    void Update()
    {
        if (enemyTarget)
        {
            Vector3 targetDirection = (enemyTarget.transform.position - transform.position).normalized;
            float speed = rb.velocity.magnitude;
            Vector3 newDirection = Vector3.RotateTowards(rb.velocity.normalized, targetDirection, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
            rb.velocity = newDirection * speed;
        }
        else
        {
            Destroy(gameObject);
        }       
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
    public void SetTarget(GameObject target)
    {
        enemyTarget = target;
    }public void SetForce(float force)
    {
        this.force = force;
    }

    private void OnTriggerEnter(Collider other)
    {
        // checks if the collider is an enemy to do damage to it
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;

            enemy.GetComponent<Enemy>().TakeDamage(damage);  
            Destroy(gameObject);
        }
    }
}
