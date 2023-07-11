using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    //FIELDS
    [SerializeField] protected int maxSpeed, minSpeed;
    protected Vector3 playerPos;
    protected Manager GameManager;
    protected int health = 1;
    protected float speed = 1;
    protected bool increase = false;
    private Vector3 m_destinationPos;
    private Rigidbody[] cachedBodies;
    protected static int aliveCount = 0;
    protected static int totalKilledCount = 0;
    protected static int killedCount = 0;
    int m_layerMask = 1 << 15;

    public bool CanMove { get; set; } = true;

    //PROPERTIES
    public static int AliveCount
    {
        get { return aliveCount; }
    }

    public static int TotalKilledCount
    {
        get { return totalKilledCount; }
    }

    public static int KilledCount
    {
        get { return killedCount; }
        set { killedCount = value; }
    }

    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public event System.Action<EnemyClass> OnDeath;
    ///////////////////////////////////////////////////////////////////////////////////

    protected virtual void Awake()
    {
        playerPos = Manager.I.Player.transform.position;
        m_destinationPos = playerPos + new Vector3(0, 0, 30);
        cachedBodies = GetComponentsInChildren<Rigidbody>();
    }

    public static void ResetStatics()
    {
        aliveCount = 0;
        killedCount = 0;
        totalKilledCount = 0;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        if (tag is "Projectile" or "PowerUpProjectile")
        {
            GameEvents.current.EnemyHit();
            Health--;
        }

        increase = tag != "PowerUpProjectile";

        if (collision.gameObject.CompareTag("Player"))
        {
            GameEvents.current.Defeat();
        }
    }

    protected float EvaluateSpeed()
    {
        if(!CanMove) return 0;
        if (transform.position.z < m_destinationPos.z) return minSpeed;
        float distance = Vector3.Distance(transform.position, m_destinationPos);
        float maxSpeed = Mathf.Min(distance, this.maxSpeed); //will prob change max speed later.
        return Mathf.Max(maxSpeed, minSpeed); //try smoothdamnping this crap
    }

    /// <summary>
    /// Adds Force to each rigidbody inside the gameobject for the ragdoll to get affected by force.
    /// </summary>
    /// <param name="force">The magnitude and dir to apply</param>
    protected void AddRagdollForce(Vector3 force)
    {
        foreach (Rigidbody rb in cachedBodies)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(force, ForceMode.Impulse);
        }
    } 

    /// <summary>
    /// Resets the velocity and angular velocity of each rigidbody inside the game object.
    /// </summary>
    protected void ResetVelocities()
    {
        foreach (Rigidbody rb in cachedBodies)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeRotation 
                             | RigidbodyConstraints.FreezePositionX 
                             | RigidbodyConstraints.FreezePositionZ;
        }
    }

    protected virtual void OnEnable()
    {
        aliveCount++;
        transform.position = Manager.I.GetEnemySpawnPoint();
        gameObject.layer = 10;
        if (Physics.Raycast(transform.position, -transform.up, out var hit, Mathf.Infinity))
        {
            transform.position = hit.point;
        }
    }

    /// <summary>
    /// Sets the Game Objects to inactive after 3 seconds.
    /// </summary>
    /// <param name="objectsToDisable">objects to disable</param>
    protected void SequenceDisappear(params GameObject[] objectsToDisable) => EnemyPooler.I.SequenceMobDisable(objectsToDisable);

    //TODO prob also want to do lil particle effects too...
    protected virtual void OnDisable()
    {
        totalKilledCount++;
        OnDeath?.Invoke(this);
    }
}