using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5); // dies after 5 seconds
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
