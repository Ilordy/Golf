using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplasher : MonoBehaviour
{
    [SerializeField] GameObject waterSplashPrefab;
    void OnCollisionEnter(Collision other)
    {
        var spawnPoint = other.GetContact(0);
        foreach (Collider col in other.transform.root.GetComponentsInChildren<Collider>())
        {
            var rb = col.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(0,-50,0);
            //rb.angularVelocity = Vector3.zero;
            Destroy(col);//fix this.
        }
        GameObject splash = Instantiate(waterSplashPrefab, spawnPoint.point, Quaternion.Euler(-90, 0, 0));
    }
}
