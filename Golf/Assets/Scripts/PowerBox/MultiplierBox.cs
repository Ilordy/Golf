using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierBox : PowerBox
{
    [SerializeField] Texture2D[] emissionMaps;
    private int multiplier;
    const string emissionProperty = "_EmissionMap";
    private int m_xOffSet = 1;

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
            Vector3 previousPosition = other.transform.position;
            for (int i = 0; i < multiplier; i++)
            {
                float xOffSet = Vector3.Distance(ballPos, previousPosition) + m_xOffSet;
                if (previousPosition.x < ballPos.x)
                    xOffSet = -xOffSet;
                Vector3 spawnPoint = new Vector3(ballPos.x + xOffSet, ballPos.y, ballPos.z);
                GameObject p = Instantiate(other.gameObject, spawnPoint, other.gameObject.transform.rotation);;
                previousPosition = p.transform.position;
                p.GetComponent<Projectile>().SetForce();
            }
        }
        base.OnCollisionEnter(other);
    }
}
