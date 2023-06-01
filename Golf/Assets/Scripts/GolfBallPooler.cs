using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallPooler : BasePooler<Projectile>
{
    [SerializeField] Projectile ballPrefab;
    void Start()
    {
        InitPool(ballPrefab);
    }

    protected override void GetSetup(Projectile obj)
    {
        base.GetSetup(obj);
        obj.OnDeath += Release;
    }
    
    protected override void ReleaseSetup(Projectile obj)
    {
        base.ReleaseSetup(obj);
        obj.OnDeath -= Release;
    }
}
