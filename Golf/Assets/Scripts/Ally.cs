using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MobileTools;

public class Ally : MonoBehaviour
{
    [SerializeField] float fireRate;
    [SerializeField] Transform clubTransform;
    [SerializeField] Vector3 clubPosition, clubRotation;
    [SerializeField] GameObject bullet;
    [SerializeField] AnimatorEventHandler eventHandler;
    private Animator animator;
    private bool isGrounded;
    private GameObject floor;
    private const string shootTrigger = "Shoot";
    readonly int shootHash = Animator.StringToHash(shootTrigger);
    readonly int landedHash = Animator.StringToHash("Landed");
    Transform currentTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
        floor = Manager.I.RunWay;
        eventHandler = GetComponent<AnimatorEventHandler>();
        eventHandler.AddCallBack(ShootBall, "Shoot Ball", 0.5f);
        eventHandler.AddCallBack(OnBallShot, "Shoot Ball", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGrounded && Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Terrain")))
        {
            if (Vector3.Distance(transform.position, hit.point) < 5f)
            {
                //offset for starting the animation.
                isGrounded = true;
                animator.SetTrigger(landedHash);
                //gotta play some sound here for falling and landing.
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(shootTrigger) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            //animator.SetBool(shootHash, false);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(shootTrigger) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            //fire ball here
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Untagged"))
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Destroy(GetComponent<Collider>());
            StartCoroutine(OnLanded());
        }
    }

    IEnumerator OnLanded()
    {
        //we wait till the animator is in transition from landed to idle.
        yield return new WaitUntil(() => animator.IsInTransition(0));
        transform.position = new Vector3(transform.position.x, 1.57f, transform.position.z);
        clubTransform.localPosition = clubPosition;
        clubTransform.localEulerAngles = clubRotation;
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(fireRate);
            var enemies = FindObjectsOfType<EnemyClass>().Where(e => e.CompareTag("Enemy")); //find all enemies still alive.
            Transform target = enemies?.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).FirstOrDefault()?.transform; //shoot at the closest one.
            if (target)
            {
                currentTarget = target;
                animator.SetBool(shootTrigger, true);
            }
        }
    }

    void ShootBall()
    {
        if (!currentTarget) return;
        var p = Manager.I.ProjectilePooler.Get();
        p.transform.position = transform.position + new Vector3(0, .65f, 0);
        p.transform.LookAt(currentTarget);
        p.Rb.useGravity = true;
    }

    void OnBallShot() => animator.SetBool(shootHash, false);
}