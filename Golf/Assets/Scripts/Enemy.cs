using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(0.015f,0,0);

        if(transform.position.x > 22) {
            Destroy(gameObject);
        }

        if(health <= 0) {
            GameObject.Find("Manager").GetComponent<Manager>().enemiesKilled++;
            Destroy(gameObject);
        }
    }

    void rewardPlayer() {
        GameObject.Find("Manager").GetComponent<Manager>().reward = Random.Range(1,15);
    }
}
