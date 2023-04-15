using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy : Enemy
{
    Transform shield;
    Rigidbody shieldRb;
    Rigidbody enemyRb;

    Animator enemyAnimator;

    protected override void Start()
    {
        base.Start();

        health = 2;
        speed = 2f;

        shield = transform.Find("shield");
        shieldRb = shield.GetComponent<Rigidbody>();
        enemyRb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponent<Animator>();
    }

    protected override void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision);

        if (shield != null) {
            shield.parent = null;
            shield.gameObject.layer = 12;
            shieldRb.useGravity = true;
            shieldRb.AddForce(transform.forward * Random.Range(5f,10f), ForceMode.Impulse);
            enemyRb.isKinematic = false;
            enemyAnimator.SetBool("run", true);
            minSpeed = 5;
            shield = null;
        }
    }
}
