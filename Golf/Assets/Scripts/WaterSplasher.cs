using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaterSplasher : MonoBehaviour
{
    public static Action OnBossTouchedWater;
    [SerializeField] GameObject waterSplashPrefab;
    void OnCollisionEnter(Collision other)
    {
        var spawnPoint = other.GetContact(0);
        foreach (Collider col in other.transform.root.GetComponentsInChildren<Collider>())
        {
            var rb = col.GetComponent<Rigidbody>();
            rb.AddForce(new Vector3(0, -50, 0), ForceMode.VelocityChange);
            Destroy(col);
        }
        GameObject splash = Instantiate(waterSplashPrefab, spawnPoint.point, Quaternion.Euler(-90, 0, 0));
        var ps = splash.GetComponent<ParticleSystem>();
        if (other.transform.root.name.StartsWith("Boss"))
        {
            OnBossTouchedWater?.Invoke();
            Debug.Log("BOSS LANDED");
            splash.transform.localScale = Vector3.one * 11.2f;
            StartCoroutine(NotifyWorldManager());
        }
        ps.Play();
        //TODO: add splash sound next ONLY IF they are visible on screen.
    }

    IEnumerator NotifyWorldManager()
    {
        yield return new WaitForSeconds(2);
        WorldManager.I.BossHitOcean();
    }
}
