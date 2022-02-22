using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartEnemy : EnemyClass {
    public GameObject[] frags;
    public GameObject regularEnemy;

    void Start() {
        AliveCount++;

        health = 3;

        speed = 0.01f;

        GameManager = GameObject.Find("Manager").GetComponent<Manager>();
        playerPos = GameObject.Find("Player").transform.position;
    }

    void Update() {
        if(health <= 0) {
            for (int i = 0; i < 3; i++) {
                Instantiate(regularEnemy,transform.position + new Vector3(Random.Range(0,3),0,Random.Range(0,3)),Quaternion.identity);
            }
            Destroy(gameObject);
            TotalKilledCount++;
            if (increase) {
                KilledCount++;
            }
        } else {
            transform.LookAt(playerPos);
            transform.Translate(0,0,speed);
        }
    }

    void FixedUpdate() {
    }

    void OnCollisionEnter(Collision collision) {
        if (health == 2) {
            frags[0].transform.SetParent(null);
            frags[0].GetComponent<Rigidbody>().useGravity = true;
            frags[0].GetComponent<Rigidbody>().AddForce(new Vector3(10,0,0), ForceMode.Impulse);
        }
        if (health == 1) {
            for (int i = 1; i < frags.Length; i++) {
                frags[i].transform.SetParent(null);
                frags[i].GetComponent<Rigidbody>().useGravity = true;
                frags[i].GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-5,-1),5,0), ForceMode.Impulse);
            }
        }
        if (collision.gameObject.tag == "PowerUpProjectile") {
            increase = false;
        } else {
            increase = true;
        }
        if (collision.gameObject.tag == "Player") {
            GameManager.HandleDefeat();
        }
    }
}
