using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBox : PowerBox
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            TimeManager.I.DoSlowmotion(5);
        }
        base.OnCollisionEnter(other);
    }
}
