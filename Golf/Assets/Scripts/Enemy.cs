using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyClass
{
    [SerializeField] float tackleRange;
    [SerializeField] EmojiController emojiController;
    bool isDead = false;
    CapsuleCollider col;
    Animator animator;
    ParticleSystem particles;
    private Rigidbody rb;
    private Material emojiMat;
    public static bool isPassive = true;
    public static event System.Action OnBecameHostile;
    
    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        particles = GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
        GameManager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    protected virtual void Start()
    {
        if (!isPassive)
            emojiController.Init();
        aliveCount++;
        health = 1;
        speed = 5f;
        animator.SetInteger("IdleID", Random.Range(0, 3));
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (isPassive) return;
        animator.SetInteger("IdleID", -1);
        speed = EvaluateSpeed();
        if (health <= 0)
        {
            totalKilledCount++;
            GameEvents.current.ProgressChange(totalKilledCount, aliveCount);
            if (increase)
            {
                killedCount++;
                if (killedCount <= 5)
                {
                    GameEvents.current.KillsChange(killedCount);
                }
            }
            health = 10000;
            isDead = true;
            gameObject.tag = "Dead";
            GameEvents.current.Reward();
            col.enabled = false;
            particles.Stop(true, UnityEngine.ParticleSystemStopBehavior.StopEmittingAndClear);
            AddRagdollForce((new Vector3(0, 1.3f, 0) + Vector3.forward) * 100);
            animator.enabled = false;
            StartCoroutine(SequenceDeath());
        }

        if (!isDead && !Manager.I.PlayerDead)
        {
            if (Vector3.Distance(transform.position, playerPos) < tackleRange)
            {
                animator.SetTrigger("Tackle");
            }
            else
            {
                transform.LookAt(playerPos);
            }
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else
            animator.SetTrigger("Victory");
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Projectile"))
            SetPassive();
        base.OnCollisionEnter(collision);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        health = 1;
        isDead = false;
        gameObject.tag = "Enemy";
        col.enabled = true;
        particles.Play(true);
        animator.enabled = true;
        animator.SetTrigger("Reset");
    }

    private void SetPassive()
    {
        if (!isPassive) return;
        isPassive = false;
        OnBecameHostile?.Invoke();
    }
}
