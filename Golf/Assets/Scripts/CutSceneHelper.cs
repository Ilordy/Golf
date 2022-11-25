using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CutSceneHelper : Singleton<CutSceneHelper>
{
    [SerializeField] CinemachineVirtualCamera mainCam, playerCam, ballCam, bossCam;
    [SerializeField] BossSliderInputHandler InputSwingUI;
    [SerializeField] CineMachineBlendHelper blendHelper;
    System.Action onPlayerCutsceneFinished;
    void OnEnable()
    {
        blendHelper.onCameraBlendFinished += OnPlayerCamBlended;
    }

    void OnDisable()
    {
        blendHelper.onCameraBlendFinished += OnPlayerCamBlended;
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
    }

    public void SetBossCamFocus(Transform focus)
    {
        bossCam.LookAt = focus;
        bossCam.Follow = focus;
        bossCam.Priority = 4;
    }

    private void OnPlayerCamBlended(ICinemachineCamera cam)
    {
        if (cam.Name != playerCam.Name) return;
        InputSwingUI.gameObject.SetActive(true);
        onPlayerCutsceneFinished?.Invoke();
    }

    protected override void InternalInit()
    {

    }

    protected override void InternalOnDestroy()
    {

    }
}
