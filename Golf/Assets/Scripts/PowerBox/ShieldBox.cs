using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBox : PowerBox
{
    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Manager.I.UpdateShield();
        }
        base.OnCollisionEnter(other);
    }
}
