using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyClass
{
    bool isDead = false;
    void Start()
    {
        AliveCount++;

        health = 1;
        
        speed = 0.025f;

        GameManager = GameObject.Find("Manager").GetComponent<Manager>();
        playerPos = GameObject.Find("Player").transform.position;
    }

    void Update()
    {
        if(health <= 0) {
            TotalKilledCount++;
            if (increase) {
                KilledCount++;
            }
            health = 10000;
            isDead = true;
            timer = Time.time;
            gameObject.tag = "Dead";
            GameManager.HandleReward();
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            gameObject.GetComponent<Animator>().enabled = false;
            gameObject.GetComponent<ParticleSystem>().Stop(true,UnityEngine.ParticleSystemStopBehavior.StopEmittingAndClear);
            Destroy(gameObject,5);
        }

        if(!isDead) {
            transform.LookAt(playerPos);
            transform.Translate(0,0,speed);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "PowerUpProjectile") {
            increase = false;
        } else {
            increase = true;
        }
    }
}
