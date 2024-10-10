using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float force;
    public GameObject enemyTarget;
    // Start is called before the first frame update
    private Rigidbody rb;
    [SerializeField]
    private float turnSpeed;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Destroy(gameObject, 5); // dies after 5 seconds
    }

    // Update is called once per frame




    void Update()
    {
        Vector3 targetDirection = (enemyTarget.transform.position - transform.position).normalized;
        float speed = rb.velocity.magnitude;       
        Vector3 newDirection = Vector3.RotateTowards(rb.velocity.normalized, targetDirection, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
        rb.velocity = newDirection * speed;
    }


    public void SetDamage(float damage)
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
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;

            //enemy.TakeDamage(damage):  
            Destroy(gameObject);
        }
    }
}
