using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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
        m_localEnemies = new Collider[GameManager.MaxBounces];
        GameEvents.current.BallHit();
        bounces = 0;
        StartCoroutine(Disable());
        rb.velocity = Vector3.zero;
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
            bounces++;
            //Physics.IgnoreCollision(col, collision.collider);
            m_targettedEnemy = null;
            // collision.gameObject.layer = 7; //Targeted Enemy Layer
            GetNearbyEnemy();
        }
        else
        {
           // gameObject.SetActive(false);
        }
    }

    void GetNearbyEnemy()
    {
        //these are all layers to avoid.
        int layerMask = 1 << 6 |
                        1 << 8 | 1 << 7 |
                        1 << 12 | 1 << 14 | 1 << 11;
        int numFound = Physics.OverlapSphereNonAlloc(transform.position, 10, m_localEnemies, ~layerMask);
        var localEnemy = m_localEnemies[Random.Range(0, numFound)];
        m_targettedEnemy = localEnemy.transform;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        m_targettedEnemy = null;
        OnDeath?.Invoke(this);
    }
    
}