using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy : Enemy
{
    GameObject shield;
    Rigidbody shieldRb;
    Rigidbody enemyRb;

    Animator enemyAnimator;

    protected override void Start()
    {
        base.Start();

        health = 2;
        speed = 2f;

        shield = transform.GetChild(2).gameObject;
        shieldRb = shield.GetComponent<Rigidbody>();
        enemyRb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision);

        if (health == 1) {
            if (shield != null) {
                shield.transform.parent = null;
                shieldRb.useGravity = true;
                shieldRb.AddForce(transform.forward * Random.Range(5f,10f), ForceMode.Impulse);
            }

            enemyRb.isKinematic = false;
            enemyAnimator.SetBool("run", true);
            speed = 3f;
        }
    }
}
