using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class TimeManager : Singleton<TimeManager>
{
    [SerializeField] float slowdownFactor = 0.05f;
    private float m_startFixedDeltaTime;
    private float slowdownLength = 5f;
    public event Action OnTimeUpdated;

    private void Start()
    {
        m_startFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        //BELOW IS FOR BULLET TIME INCASE WE WANNA USE IN FUTURE.
        // Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
        //Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        // if (Time.timeScale == 1) isSlowedDown = false;
        if (Input.GetKeyDown(KeyCode.G))
            FlipTime();
    }

    public void DoSlowmotion(int seconds)
    {
        StartCoroutine(SlowTime(seconds));
    }//make preatier.
    IEnumerator SlowTime(int seconds)
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        OnTimeUpdated?.Invoke();
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1;
        Time.fixedDeltaTime = m_startFixedDeltaTime;
        OnTimeUpdated?.Invoke();
    }

    void FlipTime()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = m_startFixedDeltaTime;
        }
    }
}