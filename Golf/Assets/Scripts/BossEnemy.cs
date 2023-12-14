using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
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
    private Rigidbody[] rbs;
    private Vector3 totalVelocity;

    public Animator Anim => anim;


    void Start()
    {
        //TODO save the boss, dont actually destroy and inatiantiate.   
        focusPoint = transform.Find("metarig/spine");
        anim = GetComponent<Animator>();
        transform.parent = null;
        rb = GetComponent<Rigidbody>();
        rbs = GetComponentsInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawLine(focusPoint.position, FindLandingPoint(focusPoint.GetComponent<Rigidbody>().velocity, focusPoint.position), Color.blue, .01f);
        // Debug.Log(focusPoint.GetComponent<Rigidbody>().velocity + " MY VELOCITY FAM");
        //Debug.Log(rb.velocity);

        //Debug.DrawLine(focusPoint.position, FindLandingPoint(rb.velocity, focusPoint.position));
        if (isStunned) return;
        if (!Manager.I.PlayerDead)
            transform.LookAt(Manager.I.Player.transform.position);
        transform.Translate(0, 0, 10 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            other.gameObject.SetActive(false);
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
        else if (other.gameObject.name.Equals("World") && health <= 0)
        {
            //FindLandingPoint(rb.velocity, focusPoint.position);
            // Debug.DrawLine(focusPoint.position, FindLandingPoint(rb.velocity, focusPoint.position), Color.blue, 10f);
            // Debug.Log(rb.velocity + " MY VELOCITY FAM");
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
        float finalForceZ = force * defaultForceZ;
        Vector3 finalForce = (-transform.forward * finalForceZ + transform.up * defaultForceY);
        Debug.DrawLine(focusPoint.position, FindLandingPoint(finalForce, focusPoint.position), Color.red, 10f);
        WorldManager.I.SpawnOceans(FindLandingPoint(finalForce, focusPoint.position));
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            this.rb.isKinematic = false;
            // rb.constraints = RigidbodyConstraints.None;
            //rb.constraints = RigidbodyConstraints.FreezePositionX;
            if (rb.gameObject.name.Equals("spine"))
            {
                //this.rb.constraints = RigidbodyConstraints.FreezePositionX;
                rb.constraints = RigidbodyConstraints.FreezePositionX;
            }

            rb.AddForce(finalForce, ForceMode.VelocityChange);
            //Debug.Log(rb.velocity + " THIS THE TOTAL VELOCITY BROOOOO");
            //if (!rb.gameObject.name.Equals("spine"))
            //  Destroy(rb.GetComponent<Collider>());
        }

        StartCoroutine(WaitFixedFrame());
    }

    IEnumerator WaitFixedFrame()
    {
        yield return new WaitForFixedUpdate();
        foreach (Rigidbody VARIABLE in GetComponentsInChildren<Rigidbody>())
        {
            totalVelocity += VARIABLE.velocity;
        }
        //Debug.Log(totalVelocity + " THIS THE TOTAL VELOCITY BROOOOO " + rb.transform);
    }

    private Vector3 FindLandingPoint(Vector3 initialVelocity, Vector3 startPos)
    {
        float theta = Mathf.Atan2(initialVelocity.y, initialVelocity.z);
        float zRange = Mathf.Pow(initialVelocity.z, 2) * (Mathf.Sin(2 * theta) / Physics.gravity.y);
        Debug.Log(new Vector3(startPos.x, startPos.y, startPos.z - zRange) + " RANGEE");
        return new Vector3(startPos.x, startPos.y, startPos.z - zRange);
    }
}