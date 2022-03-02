using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    //FIELDS
    protected Vector3 playerPos;
    protected Manager GameManager;
    protected int health = 1;
    protected float speed = 1;
    protected bool increase = false;

    protected static int aliveCount = 0;
    protected static int totalKilledCount = 0;
    protected static int killedCount = 0;

    //PROPERTIES
    public int AliveCount {get{return aliveCount;}}
    public int TotalKilledCount {get{return totalKilledCount;}}
    public int KilledCount {get{return killedCount;}set{killedCount = value;}}
    public int Health {get{return health;}set{health = value;}}

    ///////////////////////////////////////////////////////////////////////////////////

    public static void ResetStatics() {
        aliveCount = 0;
        killedCount = 0;
        totalKilledCount = 0;
    }

    protected virtual void OnCollisionEnter(Collision collision) {
        string tag = collision.gameObject.tag;

        if(tag == "Projectile" || tag == "PowerUpProjectile") GameEvents.current.EnemyHit();

        increase = tag == "PowerUpProjectile" ? false : true;

        if (collision.gameObject.tag == "Player") {
            GameEvents.current.Defeat();
        }
    }

}
