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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        GameManager = GameObject.FindObjectOfType<Manager>();
        //rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        GameEvents.current.BallHit();
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (bounces >= GameManager.MaxBounces)
        {
            Destroy(gameObject);
        }

    }

    public void SetForce()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * speed / Time.timeScale, ForceMode.Impulse);
    }

    void OnEnable()
    {
        TimeManager.I.OnTimeUpdated += SetForce;
    }
    void OnDisable()
    {
        TimeManager.I.OnTimeUpdated -= SetForce;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyClass>().Health--;
            bounces++;
        }
    }
}
