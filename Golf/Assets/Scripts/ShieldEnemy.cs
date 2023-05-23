using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy : Enemy
{
    Transform shield;
    Rigidbody shieldRb;
    Rigidbody enemyRb;
    Animator enemyAnimator;
    bool shielded = true;

    protected override void Awake()
    {
        base.Awake();
        shield = transform.Find("shield");
        shieldRb = shield.GetComponent<Rigidbody>();
        enemyRb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponent<Animator>(); //TODO refactor plsss..
    }

    protected override void Start()
    {
        base.Start();
        health = 2;
        speed = 2f;
    }

    protected override void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision);
        if (shielded) {
            shield.SetParent(null);
            //shield.gameObject.layer = 12;
            shieldRb.useGravity = true;
            shieldRb.AddForce(transform.forward * Random.Range(5f,10f), ForceMode.Impulse);
            enemyRb.isKinematic = false;
            enemyAnimator.SetBool("run", true);
            minSpeed = 5;
            shielded = false;
            StartCoroutine(SequenceDisappear(shield.gameObject));
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        shield.gameObject.SetActive(true);
        shield.parent = transform;
        shield.localPosition = new Vector3(0, 1.099f, 0.68f);
        shield.localRotation = Quaternion.Euler(0, 0, 0);
        shieldRb.useGravity = false;
        shieldRb.isKinematic = true;
        enemyRb.isKinematic = true;
        enemyAnimator.SetBool("run", false);
        shielded = true;
    }
}
