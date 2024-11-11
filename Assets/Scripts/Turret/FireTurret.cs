using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTurret : Turret
{
    public override Projectile Shoot()
    {
        Projectile projectile = base.Shoot();
        projectile.SetElement(Element.Fire);

        return projectile;
    }
}
    