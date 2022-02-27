using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    AudioManager AudioManager;
    Manager GameManager;
    Rigidbody rb;

    float speed;
    int bounces = 0;
    bool canTarget = false;
    
    GameObject nearest = null;

    void Start()
    {
        AudioManager = GameObject.FindObjectOfType<AudioManager>();
        GameManager = GameObject.FindObjectOfType<Manager>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        AudioManager.PlayBallHit();
        
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (bounces >= GameManager.GetMaxBounces()) {
            Destroy(gameObject);
        }

        if (canTarget && nearest != null) {
            transform.LookAt(nearest.transform.position);
        } else if (canTarget && nearest == null) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            canTarget = false;
            collision.gameObject.GetComponent<EnemyClass>().health--;
            bounces++;
            float dist = float.MaxValue;
            nearest = null;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies) {
                float currentDist = Vector3.Distance(transform.position, enemy.transform.position);
                if (currentDist < dist && enemy != collision.gameObject) {
                    dist = currentDist;
                    nearest = enemy;
                }
            }
            if (nearest != null) {
                canTarget = true;
                rb.velocity = Vector3.zero;
                rb.AddForce(transform.forward * 25f, ForceMode.Impulse);
            }
            //Destroy(gameObject);
        }
    }
}
