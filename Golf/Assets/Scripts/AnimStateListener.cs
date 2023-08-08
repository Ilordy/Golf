using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Animations;

namespace MobileTools
{
    /// <summary>
    /// 
    /// </summary>
    [SharedBetweenAnimators] //test this attribute pls
    public class AnimStateListener : StateMachineBehaviour
    {
        void OnValidate()
        {
            
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //animator.
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }
    }
}