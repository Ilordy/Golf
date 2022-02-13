using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpProjectile : MonoBehaviour
{
    Rigidbody rb;

    Vector3 initialPos;

    float limit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            Destroy(collision.gameObject);
            Destroy(gameObject);
            //GameObject.Find("Manager").GetComponent<Manager>().enemiesKilled++;
        }
    }
}
