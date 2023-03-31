using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CutSceneHelper : Singleton<CutSceneHelper>
{
    [SerializeField] CinemachineVirtualCamera mainCam, playerCam, ballCam, bossCam;
    [SerializeField] BossSliderInputHandler InputSwingUI;
    [SerializeField] CineMachineBlendHelper blendHelper;
    [SerializeField] GameObject[] fogs;
    System.Action onPlayerCutsceneFinished, onCamerasResetted;
    void OnEnable()
    {
        blendHelper.onCameraBlendFinished += OnPlayerCamBlended;
        blendHelper.onCameraBlendStarted += OnPlayerCamBlendStarted;
        WaterSplasher.OnBossTouchedWater += OnBossTouchedWater;
        blendHelper.onCameraBlendFinished += OnCamsReset;
    }

    void OnDisable()
    {
        blendHelper.onCameraBlendFinished -= OnPlayerCamBlended;
        blendHelper.onCameraBlendStarted -= OnPlayerCamBlendStarted;
        WaterSplasher.OnBossTouchedWater -= OnBossTouchedWater;
        blendHelper.onCameraBlendFinished -= OnCamsReset;
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
        //SwtichFogs(2);
    }

    public void SetBossCamFocus(Transform focus)
    {
        bossCam.LookAt = focus;
        bossCam.Follow = focus;
        bossCam.Priority = 4;
        //SwtichFogs(0);
    }

    public void Reset(System.Action OnResetFinished)
    {
        onCamerasResetted = OnResetFinished;
        playerCam.Priority = 0;
        ballCam.Priority = 0;
        bossCam.Priority = 0;
        //OnResetFinished?.Invoke();
    }

    void OnBossTouchedWater()
    {
        bossCam.LookAt = null;
        bossCam.Follow = null;
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

    private void OnCamsReset(ICinemachineCamera cam)
    {
        if (cam.Name != mainCam.Name) return;
        Debug.Log("CAMS RESET");
        onCamerasResetted?.Invoke();
        onCamerasResetted = null;
        WorldManager.I.ResetBanners();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log(blendHelper.IsBlending);
            CinemachineCore.UniformDeltaTimeOverride = 0;
            Camera.main.transform.DOMoveY(30, 5f);
        }
        // if (blendHelper.IsBlending && blendHelper.CurrentActiveCamera.Name.Equals(mainCam.name))
        // {
        //     if (Physics.Raycast(Camera.main.transform.position, Vector3.back, 10))
        //     {
        //         bossCam.Priority = 1;
        //         Camera.main.transform.DOMoveY(Camera.main.transform.position.y + 30, .4f);
        //     }
        // }
    }

    private IEnumerator EnablePlayerFog()
    {
        float duration = Camera.main.GetComponent<CinemachineBrain>().ActiveBlend.Duration / 2;
        yield return new WaitForSeconds(duration);
        //SwtichFogs(1);
    }

}
