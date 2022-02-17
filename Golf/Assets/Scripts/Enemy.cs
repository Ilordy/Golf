using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    Vector3 playerPos;
    Manager manager;
    bool increase = false;
    bool death = false;

    Collider[] colliders;
    Rigidbody[] bodyParts;

    float timer;

    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        playerPos = GameObject.Find("Player").transform.position;

        colliders = gameObject.GetComponentsInChildren<Collider>();
        bodyParts = gameObject.GetComponentsInChildren<Rigidbody>();
    }

    void Update()
    {

        if(transform.position.x > 22) {
            Destroy(gameObject);
            manager.enemiesKilled++;
        }

        if(health <= 0) {
            health = 10000;
            death = true;
            timer = Time.time;
            gameObject.tag = "Dead";

            manager.enemiesKilled++;
            if (increase) {
                manager.killStreak++;
            }
            manager.reward = true;
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            gameObject.GetComponent<Animator>().enabled = false;
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
}
