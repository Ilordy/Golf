using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierBox : PowerBox
{
    [SerializeField] Texture2D[] emissionMaps;
    private int multiplier;
    const string emissionProperty = "_EmissionMap";

    protected override void Start()
    {
        base.Start();
        int index = Random.Range(0, emissionMaps.Length);
        var mat = GetComponent<ParticleSystemRenderer>().material;
        multiplier = index == 0 ? 3 : 5;
        mat.SetTexture(emissionProperty, emissionMaps[index]);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Vector3 ballPos = other.transform.position;
            int zOffSet = ballPos.z > 0 ? -1 : 1;
            Vector3 spawnPoint = new Vector3(ballPos.x, ballPos.y, ballPos.z += zOffSet);
            for (int i = 0; i < multiplier; i++)
            {
                GameObject p = Instantiate(other.gameObject, spawnPoint, other.gameObject.transform.rotation);
                spawnPoint.z += zOffSet;
                p.GetComponent<Projectile>().SetForce();
            }
        }
        base.OnCollisionEnter(other);
    }
}
