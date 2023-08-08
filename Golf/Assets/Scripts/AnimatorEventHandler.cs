using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MobileTools
{
    /// <summary>
    /// Helper class used for listening to and creating new animation events.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorEventHandler : MonoBehaviour
    {
        Animator animator;
        [SerializeField, ReadOnly] AnimationClip[] animationClips; //For display purposes only
        readonly Dictionary<string, Dictionary<float, Action>> stateEvents = new();
        HashSet<AnimationClip> animationClipsHash;

        void Awake()
        {
            GetAnimationClips();
            foreach (var clip in animationClipsHash)
            {
                stateEvents.Add(clip.name, new Dictionary<float, Action>());
            }
        }

        void OnEvent(AnimationEvent animEvent)
        {
            if (animEvent.objectReferenceParameter == this)
                stateEvents[animEvent.stringParameter][animEvent.floatParameter]?.Invoke();
        }

        void OnValidate()
        {
            GetAnimationClips();
        }

        void GetAnimationClips()
        {
            //Prevent duplicate animation names.
            animator = GetComponent<Animator>();
            animationClipsHash = new HashSet<AnimationClip>(animator.runtimeAnimatorController.animationClips);
            animationClips = animationClipsHash.ToArray();
        }

        public void AddCallBack(Action callBack, string animName, float time)
        {
            if (stateEvents.ContainsKey(animName))
            {
                time = Mathf.Clamp01(time);
                if (stateEvents[animName].ContainsKey(time))
                {
                    stateEvents[animName][time] += callBack;
                }
                else
                {
                    stateEvents[animName].Add(time, callBack);
                    var clip = animationClipsHash.First(c => c.name.Equals(animName));
                    var animationEvent = new AnimationEvent()
                    {
                        functionName = nameof(OnEvent),
                        time = Mathf.Lerp(0, clip.length, time),
                        floatParameter = time,
                        stringParameter = animName,
                        messageOptions = SendMessageOptions.DontRequireReceiver,
                        objectReferenceParameter = this
                    };

                    clip.AddEvent(animationEvent);
                }
            }
            else
            {
                Debug.LogError($"No Animation Clip by the name {animName} was found! " +
                               "Please make sure your animator component has this animation and it is spelled correctly.");
            }
        }

        public void RemoveCallBack(Action callBack, string animName, float time)
        {
            if (stateEvents.ContainsKey(animName))
            {
                time = Mathf.Clamp01(time);
                if (stateEvents[animName].ContainsKey(time))
                {
                    stateEvents[animName][time] -= callBack;
                }
            }
        }
    }
}