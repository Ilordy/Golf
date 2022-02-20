using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    Vector3 playerPos;
    Manager GameManager;
    bool increase = false;
    bool death = false;

    public static int AliveCount = 0;
    public static int TotalKilledCount = 0;
    public static int KilledCount = 0;
    public static int DeadCount = 0;

    float timer;

    void Start()
    {
        AliveCount++;

        GameManager = GameObject.Find("Manager").GetComponent<Manager>();
        playerPos = GameObject.Find("Player").transform.position;
    }

    void Update()
    {

        if(transform.position.x > 22) {
            Destroy(gameObject);
            DeadCount++;
        }

        if(health <= 0) {
            TotalKilledCount++;
            if (increase) {
                KilledCount++;
            }
            health = 10000;
            death = true;
            timer = Time.time;
            gameObject.tag = "Dead";
            GameManager.HandleReward();
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            gameObject.GetComponent<Animator>().enabled = false;
            gameObject.GetComponent<ParticleSystem>().Stop(true,UnityEngine.ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (death) {
            if (Time.time - timer >= 5) {
                Destroy(gameObject);
            }
        } else {
            transform.LookAt(playerPos);
            transform.Translate(0,0,0.025f);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "PowerUpProjectile") {
            increase = false;
        } else {
            increase = true;
        }
    }

    public static void ResetStatics() {
        AliveCount = 0;
        KilledCount = 0;
        TotalKilledCount = 0;
        DeadCount = 0;
    }
}
