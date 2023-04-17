using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartEnemy : EnemyClass
{
    public GameObject[] frags;
    public GameObject regularEnemy;
    private Rigidbody[] m_fragsRB;

    protected void Start()
    {
        aliveCount++;
        m_fragsRB = new Rigidbody[frags.Length];
        CollectFragRBs();
        health = 3;
        speed = 2f;
    }

    void Update()
    {
        if (health <= 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(regularEnemy, transform.position + new Vector3(Random.Range(0, 3), 0, Random.Range(0, 3)), Quaternion.identity);
            }
            Destroy(gameObject);
            totalKilledCount++;
            GameEvents.current.ProgressChange(totalKilledCount, aliveCount + 3);
            if (increase)
            {
                killedCount++;
                if (killedCount <= 5)
                {
                    GameEvents.current.KillsChange(killedCount);
                }
            }
        }
        else if (!Manager.I.PlayerDead)
        {
            transform.LookAt(Manager.I.Player.transform.position);
            transform.Translate(0, 0, speed * Time.deltaTime);
            speed = EvaluateSpeed();
        }
    }

    void CollectFragRBs()
    {
        for (int i = 0; i < frags.Length; i++)
        {
            m_fragsRB[i] = frags[i].GetComponent<Rigidbody>();
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (!collision.gameObject.CompareTag("Projectile")) return;

        if (health == 2)
        {
            m_fragsRB[0].transform.SetParent(null);
            m_fragsRB[0].isKinematic = false;
            m_fragsRB[0].useGravity = true;
            m_fragsRB[0].AddForce(new Vector3(10, 0, 0), ForceMode.Impulse);
        }
        if (health == 1)//update the forces pls.
        {
            for (int i = 1; i < frags.Length; i++)
            {
                m_fragsRB[i].transform.SetParent(null);
                m_fragsRB[i].isKinematic = false;
                m_fragsRB[i].useGravity = true;
                m_fragsRB[i].AddForce(new Vector3(Random.Range(-5, -1), 5, 0), ForceMode.Impulse);
            }
        }
    }
}
