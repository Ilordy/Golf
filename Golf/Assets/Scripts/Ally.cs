using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Ally : MonoBehaviour
{
    [SerializeField] float fireRate;
    [SerializeField] Transform clubTransform;
    [SerializeField] Vector3 clubPosition, clubRotation;
    [SerializeField] GameObject bullet;
    private Animator animator;
    private bool isGrounded;
    private GameObject floor;
    private const string shootTrigger = "Shoot";

    void Start()
    {
        animator = GetComponent<Animator>();
        floor = Manager.I.RunWay;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGrounded && Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Default")))
        {
            if (Vector3.Distance(transform.position, hit.point) < 5f)
            {//offset for starting the animation.
                isGrounded = true;
                animator.SetTrigger("Landed");
                //gotta play some sound here for falling and landing.
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(shootTrigger) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool(shootTrigger, false);
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
    { //we wait till the animator is in transition from landed to idle.
        yield return new WaitUntil(() => animator.IsInTransition(0));
        transform.position = new Vector3(transform.position.x, 1.6f, transform.position.z);
        clubTransform.localPosition = clubPosition;
        clubTransform.localEulerAngles = clubRotation;
        //transform.eulerAngles = new Vector3(0, 90, 0);
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(fireRate);//gotta change this later on to work with slowing down time.
            var enemies = FindObjectsOfType<EnemyClass>().Where(e => e.CompareTag("Enemy"));//find all enemies still alive.
            Transform target = enemies?.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).FirstOrDefault().transform;//shoot at the closest one.
            if (target)
            {
                animator.SetBool(shootTrigger, true);
                yield return new WaitForSecondsRealtime(animator.GetCurrentAnimatorStateInfo(0).normalizedTime / 2);
                GameObject p = Instantiate(bullet, transform.position + new Vector3(0, .65f, 0), Quaternion.identity);
                p.transform.LookAt(target);
                p.GetComponent<Rigidbody>().useGravity = true;
                p.GetComponent<Projectile>().SetForce();
                //p.GetComponent<Rigidbody>().AddForce(p.transform.forward * 25f, ForceMode.Impulse);
            }
        }
    }
}
