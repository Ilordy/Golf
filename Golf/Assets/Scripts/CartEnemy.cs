using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartEnemy : EnemyClass
{
    public GameObject[] frags;
    public GameObject regularEnemy;
    private Rigidbody[] fragsRB;
    Vector3[] fragPos;

    protected void Start()
    {
        aliveCount++;
        fragsRB = new Rigidbody[frags.Length];
        fragPos = new Vector3[frags.Length];
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
            GameEvents.current.ProgressChange(totalKilledCount, aliveCount + 3);
            if (increase)
            {
                killedCount++;
                if (killedCount <= 5)
                {
                    GameEvents.current.KillsChange(killedCount);
                }
            }
            //TODO particles here maybe..
            gameObject.SetActive(false);
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
            fragPos[i] = frags[i].transform.localPosition;
            fragsRB[i] = frags[i].GetComponent<Rigidbody>();
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (!collision.gameObject.CompareTag("Projectile")) return;

        if (health == 2)
        {
            fragsRB[0].transform.SetParent(null);
            fragsRB[0].isKinematic = false;
            fragsRB[0].useGravity = true;
            fragsRB[0].AddForce(new Vector3(10, 0, 0), ForceMode.Impulse);
        }
        if (health == 1)//update the forces pls.
        {
            for (int i = 1; i < frags.Length; i++)
            {
                fragsRB[i].transform.SetParent(null);
                fragsRB[i].isKinematic = false;
                fragsRB[i].useGravity = true;
                fragsRB[i].AddForce(new Vector3(Random.Range(-5, -1), 5, 0), ForceMode.Impulse);
            }
            StartCoroutine(SequenceDisappear(frags));
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        health = 3;
        for (int i = 0; i < fragPos?.Length; i++)
        {
            frags[i].transform.SetParent(transform);
            frags[i].transform.localPosition = fragPos[i];
            fragsRB[i].isKinematic = true;
            fragsRB[i].useGravity = false;
        }
    }
}
