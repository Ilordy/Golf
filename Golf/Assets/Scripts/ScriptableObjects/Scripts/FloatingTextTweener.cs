using UnityEngine;
using System;
using TMPro;
using UnityEngine.Pool;
public abstract class FloatingTextTweener : ScriptableObject
{
    [SerializeField] protected float m_tweenTime = 1;
    [SerializeField] protected DG.Tweening.Ease easeType = DG.Tweening.Ease.Unset;
    public abstract void TweenText(TextMeshProUGUI target, Vector2 endPos, ObjectPool<TextMeshProUGUI> pool);
}
