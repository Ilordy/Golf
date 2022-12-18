using UnityEngine;
using System;
using TMPro;
using UnityEngine.Pool;
public abstract class FloatingTextTweener : ScriptableObject
{
    [SerializeField] protected float m_tweenTime = 1;
    public abstract void TweenText(TextMeshProUGUI target, Vector2 endPos);
}
