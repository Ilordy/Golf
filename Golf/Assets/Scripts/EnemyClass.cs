using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    protected Vector3 playerPos;
    protected Manager GameManager;
    protected AudioManager AudioManager;
    public int health = 1;
    protected float speed = 1;
    protected bool increase = false;

    protected static int AliveCount = 0;
    protected static int TotalKilledCount = 0;
    protected static int KilledCount = 0;

    protected virtual void Start() {
        AudioManager = GameObject.FindObjectOfType<AudioManager>();
    }

    public int GetData(string name) {
        switch (name) {
            case "AliveCount":
                return AliveCount;
            case "TotalKilledCount":
                return TotalKilledCount;
            case "KilledCount":
                return KilledCount;
            default:
                return 0;
        }
    }

    public void SetKilledCount(int value) {
        KilledCount = value;
    }

    public static void ResetStatics() {
        AliveCount = 0;
        KilledCount = 0;
        TotalKilledCount = 0;
    }

    protected virtual void OnCollisionEnter(Collision collision) {
        string tag = collision.gameObject.tag;

        if(tag == "Projectile" || tag == "PowerUpProjectile") AudioManager.PlayEnemyHit();

        if (collision.gameObject.tag == "PowerUpProjectile") {
            increase = false;
        } else {
            increase = true;
        }
        if (collision.gameObject.tag == "Player") {
            GameManager.HandleDefeat();
        }
    }

}
