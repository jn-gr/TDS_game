using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;



public class Projectile : MonoBehaviour
{
    public float damage;
    public float force;
    public GameObject target;
    public Element element;
    // Start is called before the first frame update
    private Rigidbody rb;
    private Renderer projectileRenderer;

    [SerializeField] private Material fireMaterial;
    [SerializeField] private Material waterMaterial;
    [SerializeField] private Material neutralMaterial;
    [SerializeField] private Material airMaterial;

    [SerializeField] private float turnSpeed;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        
        projectileRenderer = gameObject.GetComponent<Renderer>();
        Destroy(gameObject, 5); // dies after 5 seconds
    }

    // Update is called once per frame




    //void FixedUpdate()
    //{
    //    if (enemyTarget)
    //    {
    //        Vector3 targetDirection = (enemyTarget.transform.position - transform.position).normalized;
    //        float speed = rb.velocity.magnitude;
    //        Vector3 newDirection = Vector3.RotateTowards(rb.velocity.normalized, targetDirection, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
    //        rb.velocity = newDirection * speed;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }

    //}

    private void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards the target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * force * Time.fixedDeltaTime;

        // Optionally, align the projectile to face the target
        transform.LookAt(target.transform);
    }


    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
    public void SetForce(float force)
    {
        this.force = force;
    }
    public void SetElement(Element element)
    {
        this.element = element;
        

        
        switch (element)
        {
            case Element.Fire:
                GetComponent<Renderer>().material = fireMaterial;
                break;
            case Element.Water:
                GetComponent<Renderer>().material = waterMaterial;
                break;
            case Element.Neutral:
                GetComponent<Renderer>().material = neutralMaterial;
                break;
            case Element.Air:
                GetComponent<Renderer>().material = airMaterial;
                break;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider has a component that implements IDamageable
        NeutralEnemy damageable = other.GetComponent<NeutralEnemy>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage, element);
            Destroy(gameObject);
        }
    }

}
