using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossEnemy : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] int defaultForceZ = 80, defaultForceY = 20;
    private Animator anim;
    private bool isStunned, canBeHit = true;
    public System.Action<BossEnemy> OnKnockedOut;
    Rigidbody rb;
    private Transform focusPoint;
    private static readonly int GotHit = Animator.StringToHash("GotHit");

    void Start()
    {
        //TODO save the boss, dont actually destroy and inatiantiate.   
        focusPoint = transform.Find("metarig/spine");
        anim = GetComponent<Animator>();
        transform.parent = null;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStunned) return;
        if (!Manager.I.PlayerDead)
            transform.LookAt(Manager.I.Player.transform.position);
        transform.Translate(0, 0, 10 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            if (!canBeHit) return;
            health--;
            if (health > 0)
            {
                anim.SetTrigger(GotHit);
                isStunned = true;
                canBeHit = false;
                GameEvents.current.EnemyHit();
                StartCoroutine(HitStun());
            }
            else
            {
                isStunned = true;
                OnKnockedOut?.Invoke(this);
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            DestroyAllColliders();
            GameEvents.current.Defeat();
        }
    }

    private void DestroyAllColliders()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
            Destroy(col);
    }

    IEnumerator HitStun()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + 1f);
        isStunned = false;
        yield return new WaitForSecondsRealtime(.3f);
        canBeHit = true;
    }

    public void AddRagDollForce(float force)
    {
        GameEvents.current.EnemyHit();
        anim.enabled = false;
        CutSceneHelper.I.SetBossCamFocus(focusPoint);
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionX /* | RigidbodyConstraints.FreezePositionY */;
        float finalForceZ = force + defaultForceZ;
        Vector3 finalForce = (-transform.forward * finalForceZ + transform.up * defaultForceY);
        Debug.DrawLine(focusPoint.position, FindLandingPoint(finalForce, focusPoint.position), Color.red, 10f);
        WorldManager.I.SpawnOceans(FindLandingPoint(finalForce, focusPoint.position));
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce((-transform.forward * finalForceZ + transform.up * defaultForceY), ForceMode.VelocityChange);
            if (!rb.gameObject.name.Equals("spine"))
                Destroy(rb.GetComponent<Collider>());
        }
    }

    private Vector3 FindLandingPoint(Vector3 initialVelocity, Vector3 startPos)
    {
        Debug.Log(initialVelocity + "THIS INIT VELOCITY");
        float acceleration = Physics.gravity.magnitude;
        float theta = Mathf.Atan(initialVelocity.y / -initialVelocity.z);
        float x = (Mathf.Pow(initialVelocity.magnitude, 2) * Mathf.Sin(2 * theta)) / acceleration;
        // float time = 2 * initialVelocity.magnitude * Mathf.Sin(theta) / acceleration;
        // Debug.Log("This the time!!! " + time);
        return new Vector3(startPos.x, startPos.y, startPos.z - x);
    }
}
