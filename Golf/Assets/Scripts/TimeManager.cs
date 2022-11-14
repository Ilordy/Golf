using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class TimeManager : Singleton<TimeManager>
{
    [SerializeField] float slowdownFactor = 0.05f;
    private float slowdownLength = 5f;
    public event Action OnTimeUpdated;

    void Update()
    {
        //BELOW IS FOR BULLET TIME INCASE WE WANNA USE IN FUTURE.
        // Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
        //Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        // if (Time.timeScale == 1) isSlowedDown = false;
        if (Input.GetKeyDown(KeyCode.G))
            DoSlowmotion(5);
        //Debug.Log(Time.timeScale);
    }

    public void DoSlowmotion(int seconds)
    {
        StartCoroutine(SlowTime(5));
    }//make preatier.
    IEnumerator SlowTime(int seconds)
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        OnTimeUpdated?.Invoke();
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1;
        OnTimeUpdated?.Invoke();
    }

    protected override void InternalInit()
    {

    }

    protected override void InternalOnDestroy()
    {

    }
}