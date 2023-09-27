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
            Manager.I.CurrentMultiplier = (CurrentMultiplier)(int)Manager.I.CurrentMultiplier + 1;
        }

        base.OnCollisionEnter(other);
    }

    public enum CurrentMultiplier
    {
        None,
        Three,
        Five
    }
}