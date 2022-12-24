using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Pool;

namespace MobileTools
{
    [CreateAssetMenu(menuName = "FloatingTextTweeners/TextScale")]
    public class FloatingTextScale : FloatingTextTweener
    {
        [SerializeField] ScaleSettings scaleSettings;
        private Vector2 m_destinationPoint;
        ObjectPool<TextMeshProUGUI> m_pool;
        public override void TweenText(TextMeshProUGUI target, Vector2 endPos, ObjectPool<TextMeshProUGUI> pool)
        {
            m_destinationPoint = endPos;
            m_pool = pool;
            target.transform.localScale = Vector3.one;
            switch (scaleSettings.scaleStyle)
            {
                case ScaleSettings.ScaleStyle.ScaleOnSpawn:
                    ScaleOnEnter(target);
                    break;
                case ScaleSettings.ScaleStyle.ScaleOnExit:
                    ScaleOnExit(target);
                    break;
                case ScaleSettings.ScaleStyle.ScaleOnBoth:
                    ScaleOnBoth(target);
                    break;
            }
        }

        private void ScaleOnEnter(TextMeshProUGUI target)
        {
            target.transform.DOScale(scaleSettings.ScaleTo, scaleSettings.scaleTweenTime).SetEase(scaleSettings.scaleEasingType);
            target.transform.DOLocalMove(m_destinationPoint, m_tweenTime).SetEase(easeType).OnComplete(() => m_pool.Release(target));
        }

        private void ScaleOnExit(TextMeshProUGUI target)
        {
            target.transform.DOLocalMove(m_destinationPoint, m_tweenTime).SetEase(easeType).OnComplete(() =>
            {
                target.transform.DOScale(scaleSettings.ScaleTo, scaleSettings.scaleTweenTime).SetEase(scaleSettings.scaleEasingType)
                .OnComplete(() => m_pool.Release(target));
            });
        }

        private void ScaleOnBoth(TextMeshProUGUI target)
        {
            target.transform.DOScale(scaleSettings.ScaleTo, scaleSettings.scaleTweenTime).SetEase(scaleSettings.scaleEasingType);
            target.transform.DOLocalMove(m_destinationPoint, m_tweenTime).SetEase(easeType).OnComplete(() =>
            {
                target.transform.DOScale(Vector3.one, scaleSettings.scaleTweenTime).SetEase(scaleSettings.scaleEasingType)
                .OnComplete(() => m_pool.Release(target));
            });
        }

        [System.Serializable]
        private class ScaleSettings
        {
            public enum ScaleStyle { ScaleOnSpawn, ScaleOnExit, ScaleOnBoth }
            public ScaleStyle scaleStyle;
            public Ease scaleEasingType;
            public float scaleTweenTime;
            public Vector2 ScaleTo;
        }
    }
}
