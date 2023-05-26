using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour
{
    Manager GameManager;
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed = 25f;
    [SerializeField] TrailRenderer trail;
    int bounces = 0;
    Vector3 initialVelocity = Vector3.zero;
    private Collider[] m_localEnemies = new Collider[5]; //Max nearby enemies it can find.
    private Transform m_targettedEnemy;
    public event Action<Projectile> OnDeath;

    public TrailRenderer Trail
    {
        get => trail;
        set => trail = value;
    }

    void Start()
    {
        GameManager = Manager.I;
        GameEvents.current.BallHit();
    }

    void OnEnable()
    {
        GameEvents.current.BallHit();
        bounces = 0;
        StartCoroutine(Disable());
    }

    IEnumerator Disable()
    {
        yield return new WaitForSecondsRealtime(5f);
        gameObject.SetActive(false);
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
            collision.gameObject.layer = 7; //Targeted Enemy Layer
            GetNearbyEnemy();
        }
    }

    void GetNearbyEnemy()
    {
        //these are all layers to avoid.
        int layerMask = 1 << 6 | 1 << 8 | 1 << 7 | 1 << 12 | 1 << 14 | 1 << 11;
        int numFound = Physics.OverlapSphereNonAlloc(transform.position, 10, m_localEnemies, ~layerMask);
        if (numFound == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        var localEnemy = m_localEnemies[Random.Range(0, numFound)];
        m_targettedEnemy = localEnemy.transform;
    }

    void OnDisable()
    {
        m_targettedEnemy = null;
        OnDeath?.Invoke(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 10);
    }
}