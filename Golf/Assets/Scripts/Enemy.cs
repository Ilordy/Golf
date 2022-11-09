using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyClass {
    bool isDead = false;
    CapsuleCollider col;
    Animator animator;
    ParticleSystem particles;
    private Rigidbody rb;
    bool isAlly;
    Transform allyTarget;

    protected virtual void Start() {
        aliveCount++;
        health = 1;
        speed = 5f;

        playerPos = GameObject.Find("Player").transform.position;
        col = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        particles = GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
        GameManager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    protected virtual void Update() {
        if (health <= 0) {
            totalKilledCount++;
            GameEvents.current.ProgressChange(totalKilledCount, aliveCount);
            if (increase) {
                killedCount++;
                if (killedCount <= 5) {
                    GameEvents.current.KillsChange(killedCount);
                }
            }
            health = 10000;
            isDead = true;
            gameObject.tag = "Dead";
            GameEvents.current.Reward();
            col.enabled = false;
            particles.Stop(true, UnityEngine.ParticleSystemStopBehavior.StopEmittingAndClear);
            if (isAlly) return;
            AddRagdollForce((new Vector3(0, 1.3f, 0) + -transform.forward) * 100);
            animator.enabled = false;
            Destroy(gameObject, 5);
        }

        if (!isDead) {
            transform.LookAt(playerPos);
            transform.Translate(0, 0, speed * Time.deltaTime);
        } else if (isAlly) {
            transform.LookAt(allyTarget);
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Converts enemy to a possible ally
    /// </summary>
    public void MakeAlly(Transform allyTarget) {
        GetComponent<Rigidbody>().isKinematic = true;
        this.allyTarget = allyTarget;
        isAlly = true;
    }

    protected override void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision);
    }
}
