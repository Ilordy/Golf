using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;

    Vector3 initialPos;

    float speed;

    float limit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 25f;
        limit = 300f;

        initialPos = transform.position;
    }

    void Update()
    {
        rb.useGravity = false;

        if (Vector3.Distance(initialPos, transform.position) > limit) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            rewardPlayer();
            Destroy(collision.gameObject);
            Destroy(gameObject);
            GameObject.Find("Manager").GetComponent<Manager>().enemiesKilled++;
        }
    }

    void rewardPlayer() {
        GameObject.Find("Manager").GetComponent<Manager>().reward = Random.Range(1,15);
    }
}
