using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5); // dies after 5 seconds
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}
