using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Manager GameManager;
    Rigidbody rb;

    float speed;
    int bounces = 0;

    void Start()
    {
        GameManager = GameObject.FindObjectOfType<Manager>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        GameEvents.current.BallHit();
        
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (bounces >= GameManager.MaxBounces) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            collision.gameObject.GetComponent<EnemyClass>().Health--;
            bounces++;
        }
    }
}
