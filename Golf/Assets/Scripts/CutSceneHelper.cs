using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CutSceneHelper : Singleton<CutSceneHelper>
{
    [SerializeField] CinemachineVirtualCamera mainCam, playerCam, ballCam, bossCam;
    [SerializeField] BossSliderInputHandler InputSwingUI;
    [SerializeField] CineMachineBlendHelper blendHelper;
    [SerializeField] GameObject[] fogs;
    System.Action onPlayerCutsceneFinished;
    void OnEnable()
    {
        blendHelper.onCameraBlendFinished += OnPlayerCamBlended;
        blendHelper.onCameraBlendStarted += OnPlayerCamBlendStarted;
    }

    void OnDisable()
    {
        blendHelper.onCameraBlendFinished -= OnPlayerCamBlended;
        blendHelper.onCameraBlendStarted -= OnPlayerCamBlendStarted;
    }

    public void DoPlayerCutScene(System.Action onCameraBlended)
    {
        playerCam.Priority = 2;
        onPlayerCutsceneFinished = onCameraBlended;

    }

    public void SetBallCamFocus(Transform focus)
    {
        ballCam.LookAt = focus;
        ballCam.Follow = focus;
        ballCam.Priority = 3;
        SwtichFogs(2);
    }

    public void SetBossCamFocus(Transform focus)
    {
        bossCam.LookAt = focus;
        bossCam.Follow = focus;
        bossCam.Priority = 4;
        SwtichFogs(0);
    }

    private void OnPlayerCamBlended(ICinemachineCamera cam)
    {
        if (cam.Name != playerCam.Name) return;
        //SwtichFogs(1);
        InputSwingUI.gameObject.SetActive(true);
        onPlayerCutsceneFinished?.Invoke();
    }

    private void OnPlayerCamBlendStarted(ICinemachineCamera cam)
    {
        if (cam.Name != playerCam.Name) return;
        StartCoroutine(EnablePlayerFog());
    }

    private void SwtichFogs(int fogIndex)
    {
        for (int i = 0; i < fogs.Length; i++)
        {
            if (i == fogIndex)
                fogs[i].SetActive(true);
            else
                fogs[i].SetActive(false);
        }
    }

    private IEnumerator EnablePlayerFog()
    {
        float duration = Camera.main.GetComponent<CinemachineBrain>().ActiveBlend.Duration / 2;
        yield return new WaitForSeconds(duration);
        SwtichFogs(1);
    }

    protected override void InternalInit()
    {

    }

    protected override void InternalOnDestroy()
    {

    }
}
