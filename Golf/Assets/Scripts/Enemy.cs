using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(0.01f,0,0);

        if(transform.position.x > 22) {
            Destroy(gameObject);
            GameObject.Find("Manager").GetComponent<Manager>().enemiesAlive--;
        }
    }
}
