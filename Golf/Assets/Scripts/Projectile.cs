using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class Projectile : MonoBehaviour
{
    Manager GameManager;
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed = 25f;
    [SerializeField] TrailRenderer trail;
    int bounces = 0;
    private Collider[] m_localEnemies; //Max nearby enemies it can find.
    private Transform m_targettedEnemy;
    private Transform lastTargettedEnemy;
    //private HashSet<Collider> collidedEnemies = new HashSet<Collider>();
    private Collider[] collidedEnemies;
    private Collider col;
    public event Action<Projectile> OnDeath;

    public TrailRenderer Trail
    {
        get => trail;
        set => trail = value;
    }

    public Rigidbody Rb => rb;


    private void Awake()
    {
        GameManager = Manager.I;
        GameEvents.current.BallHit();
        col = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        ReEnableCollisions();
        m_localEnemies = new Collider[GameManager.MaxBounces];
        collidedEnemies = new Collider[GameManager.MaxBounces];
        GameEvents.current.BallHit();
        bounces = 0;
        StartCoroutine(Disable());
        //collidedEnemies.Clear();
    }

    private void ReEnableCollisions()
    {
        if (collidedEnemies == null)
        {
            return;
        }

        foreach (var enemy in collidedEnemies)
        {
            if (enemy != null)
            {
                Physics.IgnoreCollision(col, enemy, false);
            }
        }
    }

    IEnumerator Disable()
    {
        yield return new WaitForSecondsRealtime(5f);
        gameObject.SetActive(false);
    }


    void Update()
    {
        if (bounces == GameManager.MaxBounces)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

        if (m_targettedEnemy)
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
            collidedEnemies[bounces] = collision.collider;
            bounces++;
            Physics.IgnoreCollision(col, collision.collider);
            lastTargettedEnemy = m_targettedEnemy;
            m_targettedEnemy = null;
            // collision.gameObject.layer = 7; //Targeted Enemy Layer
            GetNearbyEnemy();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void GetNearbyEnemy()
    {
        //these are all layers to avoid.
        int layerMask = 1 << 6 |
                        1 << 8 | 1 << 7 |
                        1 << 12 | 1 << 14 | 1 << 11;
        int numFound = Physics.OverlapSphereNonAlloc(transform.position, 10, m_localEnemies, ~layerMask);
        for (int i = 0; i < numFound; i++)
        {
            if (m_localEnemies[i] == collidedEnemies[i])
            {
                continue;
            }

            m_targettedEnemy = m_localEnemies[i].transform;
        }

        //var localEnemy = m_localEnemies.FirstOrDefault(c => c != null && c.transform != lastTargettedEnemy);
        if (numFound == 0 || m_targettedEnemy == null)
        {
            //BUG: IT KEEPS HITTING THE SAME ENEMY PLS FIX
            gameObject.SetActive(false);
            return; 
        }


        var localEnemy = m_localEnemies[Random.Range(0, numFound)];
        //
        // if (localEnemy.transform == lastTargettedEnemy)
        // {
        //     GetNearbyEnemy();
        //     return;
        // }
        m_targettedEnemy = localEnemy.transform;
    }

    void OnDisable()
    {
        m_targettedEnemy = null;
        OnDeath?.Invoke(this);
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position, 10);
    }
}