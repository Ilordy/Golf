using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testdebug : MonoBehaviour
{
    [SerializeField] private bool debug;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //Debug.Log(GetComponent<Rigidbody>().velocity + " VEL");
        // StartCoroutine(Next());
    }

    private Vector3 totalVelocity;
    private void FixedUpdate()
    {
        foreach (Rigidbody VARIABLE in GetComponentsInChildren<Rigidbody>())
        {
            //totalVelocity += VARIABLE.velocity;
            Debug.Log(VARIABLE.velocity + " VELL");
        }
       
        totalVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // if (debug)
        //     Debug.Log(transform.position);
        // if (Input.GetKey(KeyCode.Space))
        // {
        //    VFXManager.PlayEnemyDeathVFX(transform.position);
        // }
    }
}