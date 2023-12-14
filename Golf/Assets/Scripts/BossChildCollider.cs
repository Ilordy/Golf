using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChildCollider : MonoBehaviour
{
    private Rigidbody rb;
    private BossEnemy bossEnemy;
    private bool initialized;
    private float t;
    private Vector3 startVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bossEnemy = GetComponentInParent<BossEnemy>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!initialized && !bossEnemy.Anim.enabled && other.gameObject.layer == 6)
        {
            initialized = true;
            return;
            startVelocity = rb.velocity;
            startVelocity.y /= 4;
            startVelocity.z /= 4;
            rb.velocity = startVelocity;
        }
    }

    private void Update()
    {
        return;
        Debug.Log(rb.velocity);
        if (!initialized)
        {
            return;
        }

        // if (t < 1)
        // {
        var sourceVel = Vector3.LerpUnclamped(startVelocity, Vector3.zero, t);
        sourceVel.z = Mathf.Clamp(sourceVel.z, 0, startVelocity.z);
        rb.velocity = sourceVel;

        t += Time.deltaTime / 3;
        //}
    }
}