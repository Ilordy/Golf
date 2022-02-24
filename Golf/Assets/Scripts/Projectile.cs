using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    AudioManager AudioManager;

    Rigidbody rb;

    Vector3 initialPos;

    float speed;

    float limit;

    void Start()
    {
        AudioManager = GameObject.FindObjectOfType<AudioManager>();

        rb = GetComponent<Rigidbody>();
        limit = 300f;

        initialPos = transform.position;

        AudioManager.PlayBallHit();
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
            collision.gameObject.GetComponent<EnemyClass>().health--;
            Destroy(gameObject);
        }
    }
}
