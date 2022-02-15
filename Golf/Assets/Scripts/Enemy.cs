using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    Vector3 playerPos;
    Manager manager;
    bool increase = false;

    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        playerPos = GameObject.Find("Player").transform.position;
    }

    void Update()
    {
        transform.LookAt(playerPos);
        transform.Translate(0,0,0.025f);

        if(transform.position.x > 22) {
            Destroy(gameObject);
            manager.enemiesKilled++;
        }

        if(health <= 0) {
            manager.enemiesKilled++;
            if (increase) {
                manager.killStreak++;
            }
            rewardPlayer();
            Destroy(gameObject);
        }
    }

    void rewardPlayer() {
        manager.reward = Random.Range(1,15);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "PowerUpProjectile") {
            increase = false;
        } else {
            increase = true;
        }
    }
}
