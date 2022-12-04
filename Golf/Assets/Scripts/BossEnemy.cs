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

    void Start()
    {
        focusPoint = transform.Find("metarig/spine");
        anim = GetComponent<Animator>();
        transform.parent = null;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStunned) return;
        transform.LookAt(Manager.I.Player.transform.position);
        transform.Translate(0, 0, 10 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            if (!canBeHit) return;
            health--;
            if (health > 0)
            {
                anim.SetTrigger("GotHit");
                isStunned = true;
                canBeHit = false;
                StartCoroutine(HitStun());
            }
            else
            {
                isStunned = true;
                OnKnockedOut?.Invoke(this);
            }
        }
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
        }
    }

    private Vector3 FindLandingPoint(Vector3 initialVelocity, Vector3 startPos)
    {
        float acceleration = Physics.gravity.magnitude;
        float theta = Mathf.Atan(initialVelocity.y / initialVelocity.x);
        float x = (Mathf.Pow(initialVelocity.magnitude, 2) * Mathf.Sin(2 * theta)) / acceleration;
        return new Vector3(startPos.x + x, 0, 0);
    }
}
