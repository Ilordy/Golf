using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossEnemy : MonoBehaviour
{
    [SerializeField] int health;
    private Animator anim;
    private bool isStunned, canBeHit = true;
    public System.Action<BossEnemy> OnKnockedOut;
    Rigidbody rb;

    void Start()
    {
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
        CutSceneHelper.I.SetBossCamFocus(transform.Find("metarig/spine"));
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionX /* | RigidbodyConstraints.FreezePositionY */;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce((-transform.forward * (force + 80) + (transform.up * 20)), ForceMode.Impulse);
        }
    }
}
