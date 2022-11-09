using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBox : PowerBox {


    protected override void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Projectile")) {
            Manager.I.SpawnAlly();
        }
        base.OnCollisionEnter(other);
    }
}
