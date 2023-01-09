using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class PowerBox : MonoBehaviour
{
    private GameObject runWay, player;

    private Vector3 centorPoint, startRelCenter, endRelCenter;
    private float journeyTime = 1;
    [SerializeField] float speed;
    private float arc;
    private Vector3 startPos, endPos, maxPos, minPos;
    private float startTime;
    protected Collider runWayCollider;
    private bool reversed = false;
    private float x, y, z = 0;
    public UnityEvent OnDestroyed;

    protected virtual void Start()
    {
        runWay = Manager.I.RunWay;
        player = Manager.I.Player;
        SetRandomLightColor();
        runWayCollider = runWay.GetComponent<Collider>();
        maxPos = runWayCollider.bounds.max - player.transform.position;
        minPos = runWayCollider.bounds.min;
        x = runWayCollider.bounds.max.x;
        InitMoving();
    }

    void SetRandomLightColor()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = Random.ColorHSV(0, 1, .5f, .5f, 1, 1);
        ps.Play();
    }

    void InitMoving()
    {
        startPos = transform.position;
        y = 1.5f;
        z = startPos.z > 0 ? minPos.z : maxPos.z;
        if (startPos.x <= minPos.x + 15f && !reversed)
        {
            x = maxPos.x;
            reversed = true;
        }
        if (startPos.x >= maxPos.x - 1.5f)
        {
            x = minPos.x;
            reversed = false;
        }
        endPos = new Vector3(x, y, z);
        startTime = Time.time;
        arc = Vector3.Distance(startPos, endPos) / 2;
        journeyTime = arc;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        centorPoint = (startPos + endPos) * .5f;
        centorPoint = startPos.z > 0 ? (centorPoint + Vector3.forward) + new Vector3(0, 0, arc) : centorPoint - Vector3.forward - new Vector3(0, 0, arc);
        startRelCenter = startPos - centorPoint;
        endRelCenter = endPos - centorPoint;
        float fracComplete = (Time.time - startTime) / journeyTime * speed;
        transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
        transform.position += centorPoint;
        Debug.DrawLine(transform.position, endPos, Color.red, .1f);
        if (Vector3.Distance(transform.position, endPos) <= 0.001f)
            InitMoving();
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        //play sound and destroy
        if (other.gameObject.CompareTag("Untagged"))
        {
            InitMoving();
        }
        else if (other.gameObject.CompareTag("Projectile"))
        {
            //play particle and sound.
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        OnDestroyed?.Invoke();
        OnDestroyed.RemoveAllListeners();
    }
}
