using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyClass {
    bool isDead = false;

    protected override void Start()
    {
        base.Start();

        AudioManager = GameObject.FindObjectOfType<AudioManager>();

        AliveCount++;

        health = 1;
        
        speed = 3f;

        GameManager = GameObject.Find("Manager").GetComponent<Manager>();
        playerPos = GameObject.Find("Player").transform.position;
    }

    protected virtual void Update()
    {
        if(health <= 0) {
            TotalKilledCount++;
            if (increase) {
                KilledCount++;
            }
            health = 10000;
            isDead = true;
            gameObject.tag = "Dead";
            GameManager.HandleReward();
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
