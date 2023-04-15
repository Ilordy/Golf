using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    //FIELDS
    [SerializeField] protected int maxSpeed, minSpeed;
    protected Vector3 playerPos;
    protected Manager GameManager;
    protected int health = 1;
    protected float speed = 1;
    protected bool increase = false;
    private Vector3 m_destinationPos;

    protected static int aliveCount = 0;
    protected static int totalKilledCount = 0;
    protected static int killedCount = 0;

    //PROPERTIES
    public int AliveCount { get { return aliveCount; } }
    public int TotalKilledCount { get { return totalKilledCount; } }
    public int KilledCount { get { return killedCount; } set { killedCount = value; } }
    public int Health { get { return health; } set { health = value; } }

    ///////////////////////////////////////////////////////////////////////////////////


    void Awake()
    {
        playerPos = Manager.I.Player.transform.position;
        int layerMask = 1 << 15;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, layerMask))
        {
            transform.position = hit.point;
        }
        m_destinationPos = playerPos + new Vector3(0, 0, 30);
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

        if (tag == "Projectile" || tag == "PowerUpProjectile") GameEvents.current.EnemyHit();

        increase = tag == "PowerUpProjectile" ? false : true;

        if (collision.gameObject.tag == "Player")
        {
            GameEvents.current.Defeat();
            if (TryGetComponent(out Rigidbody rb))
                rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    protected float EvaluateSpeed()
    {
        if (transform.position.z < m_destinationPos.z) return minSpeed;
        float distance = Vector3.Distance(transform.position, m_destinationPos);
        float maxSpeed = Mathf.Min(distance, this.maxSpeed);//will prob change max speed later.
        return Mathf.Max(maxSpeed, minSpeed);
    }

    /// <summary>
    /// Adds Force to each rigidbody inside the gameobject for the ragdoll to get affected by force.
    /// </summary>
    /// <param name="force">The magnitude and dir to apply</param>
    protected void AddRagdollForce(Vector3 force)
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

}
