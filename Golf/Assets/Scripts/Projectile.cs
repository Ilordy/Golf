using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;
    float speed;

    float timer;
    float limit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 25f;
        limit = 10f;
        timer = limit;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        //rb.velocity = transform.forward * speed;
        rb.useGravity = false;

        if (timer <= 0) {
            timer = limit;
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            GameObject.Find("Manager").GetComponent<Manager>().enemiesKilled++;
        }
    }
}
