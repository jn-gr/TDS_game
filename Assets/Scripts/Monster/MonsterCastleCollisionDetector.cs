using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCastleCollisionDetector : MonoBehaviour
{
    // When monster collider collides with object with 'Castle' tag, it sends a log to console.
    // Does nothing yet.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Castle"))
        {
            Debug.Log("castle has been hit by monster");
        }
    }
}
