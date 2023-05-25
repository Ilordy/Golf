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
}
