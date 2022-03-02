using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyClass {
    bool isDead = false;

    protected virtual void Start()
    {
        aliveCount++;

        health = 1;
        
        speed = 3f;

        playerPos = GameObject.Find("Player").transform.position;
    }

    protected virtual void Update()
    {
        if(health <= 0) {
            totalKilledCount++;
            if (increase) {
                killedCount++;
            }
            health = 10000;
            isDead = true;
            gameObject.tag = "Dead";
            GameEvents.current.Reward();
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            gameObject.GetComponent<Animator>().enabled = false;
            gameObject.GetComponent<ParticleSystem>().Stop(true,UnityEngine.ParticleSystemStopBehavior.StopEmittingAndClear);
            Destroy(gameObject,5);
        }

        if(!isDead) {
            transform.LookAt(playerPos);
            transform.Translate(0,0,speed * Time.deltaTime);
        }
    }

    protected override void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision);
    }
}
