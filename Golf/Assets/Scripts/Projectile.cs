using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Manager GameManager;
    Rigidbody rb;
    [SerializeField] float speed = 25f;
    int bounces = 0;
    Vector3 initialVelocity = Vector3.zero;
    private Collider[] m_localEnemies = new Collider[5];//Max nearby enemies it can find.
    private Transform m_targettedEnemy;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        GameManager = Manager.I;
        //rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        GameEvents.current.BallHit();
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (bounces > GameManager.MaxBounces)
        {
            Destroy(gameObject);
        }
        if (m_targettedEnemy != null)
        {
            transform.LookAt(m_targettedEnemy);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed / Time.timeScale;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            bounces++;
            m_targettedEnemy = null;
            collision.gameObject.layer = 7; //TargettedEnemy Layer
            GetNearbyEnemy();
        }
    }

    void GetNearbyEnemy()
    {
        //these are all layers to avoid.
        int layermask = 1 << 6 | 1 << 8 | 1 << 7 | 1 << 12 | 1 << 14 | 1 << 11; 
        int numFound = Physics.OverlapSphereNonAlloc(transform.position, 10, m_localEnemies, ~layermask);
        if (numFound == 0)
        {
            Destroy(gameObject);
            return;
        }
        var localEnemy = m_localEnemies[Random.Range(0, numFound)];
        m_targettedEnemy = localEnemy.transform;
        //Mathf.log
        //SetForce();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 10);
    }
}
