using UnityEngine;
using Cinemachine;
public class CineMachineBlendOffSet : CinemachineExtension
{
    [Header("Animation is only triggerd when other camera name contains this value")]
    public string otherCamNameToTrigger = "Engine";
    [Header("Aniamtion parameter (Curve should start and end at 0, with peak at 1)")]
    public AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[]
        {
            new Keyframe(0f,0f),
            new Keyframe(0.5f,1f),
            new Keyframe(1f,0f),
        });
    public float heightOffset = 5f;
    private Vector3 _offset = Vector3.zero;
    private CinemachineBrain _brain;

 
    //This should only ever be called if the cam is either active or in transtion
    private void Update()
    {
        if(!VerifyBrain())
        {
            return;
        }
        //Reset offset
        _offset = Vector3.zero;
   
        //Is a blend currently happening
        if (_brain.ActiveBlend != null && !_brain.ActiveBlend.IsComplete)
        {
            var blend = _brain.ActiveBlend;
            if (blend.CamA != null && blend.CamB != null)
            {
                //Sould animate?
                if (ShouldAnimate(blend.CamA, blend.CamB))
                {
                    //Evaluate blend curve
                    float lerp = Mathf.Clamp01(blend.TimeInBlend / blend.Duration);
                    _offset = offsetCurve.Evaluate(lerp) * heightOffset * Vector3.up;
                }
            }
        }
    }
 
    //CinemachineExtension Interface method
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Noise)
        {
            state.PositionCorrection += _offset;
        }
        
    }
 
    private bool VerifyBrain()
    {
        if (!_brain)
        {
            _brain = CinemachineCore.Instance.FindPotentialTargetBrain(VirtualCamera);
            return _brain != null;
        }
        return true;
    }
 
    private bool ShouldAnimate(ICinemachineCamera camA, ICinemachineCamera camB)
    {
        GameObject otherCam;
        if(camA.VirtualCameraGameObject == VirtualCamera.gameObject)
        {
            otherCam = camB.VirtualCameraGameObject;
        }
        else if(camB.VirtualCameraGameObject == VirtualCamera.gameObject)
        {
            otherCam = camA.VirtualCameraGameObject;
        }
        //Camera isn't part of the active blend?
        else
        {
            return false;
        }
        //Check for name match
        return otherCam.name.Contains(otherCamNameToTrigger);
    }
}
 