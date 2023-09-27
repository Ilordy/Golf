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
    private Vector3 startPos, endPos, maxPos, minPos, finalPos;
    private float startTime;
    private Collider runWayCollider;
    private bool reversed = false;
    private float x, y, z = 0;
    public UnityEvent OnDestroyed;
    private Camera mainCam;

    protected virtual void Start()
    {
        mainCam = Camera.main;
        runWay = Manager.I.RunWay;
        player = Manager.I.Player;
        SetRandomLightColor();
        runWayCollider = runWay.GetComponent<Collider>();
        maxPos = runWayCollider.bounds.max - player.transform.position;
        minPos = runWayCollider.bounds.min;
        x = runWayCollider.bounds.max.x;
        SetMinMax();
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
        finalPos = GetFinalPosition();
        startPos = transform.position;
        //reversed = false;
        y = 1.995f;
        z = finalPos.z;
        x = startPos.x <= minPos.x ? maxPos.x : minPos.x;
        if (startPos.z <= minPos.z + 1 && !reversed)
        {
            reversed = true;
        }

        if (startPos.z >= maxPos.z && reversed)
        {
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
        SetMinMax();
    }

    void SetMinMax()
    {
        minPos = new Vector3(mainCam.ViewportToWorldPoint(new Vector3(0.05f, 0,
                Mathf.Abs(mainCam.transform.position.z - transform.position.z))).x,
            1.8f,
            -76);
        maxPos = new Vector3(mainCam.ViewportToWorldPoint(new Vector3(0.95f, 0,
                Mathf.Abs(mainCam.transform.position.z - transform.position.z))).x,
            1.8f,
            -25); //hard coded values due to map being static.
    }

    private void Move()
    {
        centorPoint = (startPos + endPos) * .5f;
        if (startPos.x >= minPos.x && finalPos.z == minPos.z)
        {
            centorPoint = (centorPoint - Vector3.forward) - new Vector3(arc, 0, 0);
        }
        else if (startPos.x <= maxPos.x && finalPos.z == minPos.z)
        {
            centorPoint = centorPoint - Vector3.forward - new Vector3(arc, 0, 0);
        }
        else if (startPos.x <= maxPos.x && finalPos.z == maxPos.z) //going back
        {
            centorPoint = centorPoint - Vector3.forward - new Vector3(arc, 0, 0);
        }
        else if (startPos.x >= minPos.x && finalPos.z == minPos.z) //going forward
        {
            centorPoint = (centorPoint - Vector3.forward) - new Vector3(arc, 0, 0);
        }

        startRelCenter = startPos - centorPoint;
        endRelCenter = endPos - centorPoint;
        float fracComplete = (Time.time - startTime) / journeyTime * speed;
        transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
        transform.position += centorPoint;
        Debug.DrawLine(transform.position, endPos, Color.red, .1f);
        if (Vector3.Distance(transform.position, endPos) <= 0.001f || transform.position.x > maxPos.x || transform.position.x < minPos.x)
            InitMoving();
    }

    private Vector3 GetFinalPosition()
    {
        return reversed == false ? minPos : maxPos;
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
        OnDestroyed?.RemoveAllListeners();
    }
}