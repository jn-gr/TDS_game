using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

public class FireEnemy : Enemy
{
    public override void Start()
    {
        base.Start();
        element = Element.Fire;
    }
    public override void TakeDamage(int damage, Element element)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            gameManager.EnemyKilled();
            Destroy(gameObject);
        }
    }
}
