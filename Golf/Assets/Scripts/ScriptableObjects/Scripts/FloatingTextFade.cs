using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Pool;

namespace MobileTools
{
    [CreateAssetMenu(menuName = "FloatingTextTweeners/AlphaFade")]
    public class FloatingTextFade : FloatingTextTweener
    {
        #region Variables
        private enum FadeType { OnSpawn, OnExit, Both }
        [SerializeField] float m_fadeTweenTime = 1;
        [SerializeField] FadeType m_fadeType;
        private Vector2 m_destinationPoint;
        ObjectPool<TextMeshProUGUI> m_pool;
        #endregion

        #region Methods
        public override void TweenText(TextMeshProUGUI target, Vector2 endPos, ObjectPool<TextMeshProUGUI> pool)
        {
            m_destinationPoint = endPos;
            m_pool = pool;
            target.color = new Color(target.color.r, target.color.g, target.color.b, 1);
            switch (m_fadeType)
            {
                case FadeType.OnSpawn:
                    FadeIn(target);
                    break;
                case FadeType.OnExit:
                    FadeOut(target);
                    break;
                case FadeType.Both:
                    FadeInAndOut(target);
                    break;
            }
        }

        private void FadeIn(TextMeshProUGUI target)
        {
            target.color = new Color(target.color.r, target.color.g, target.color.b, 0);
            target.DOFade(1, m_fadeTweenTime);
            target.transform.DOLocalMove(m_destinationPoint, m_tweenTime).SetEase(easeType).OnComplete(() => m_pool.Release(target));
        }

        private void FadeOut(TextMeshProUGUI target)
        {
            target.transform.DOLocalMove(m_destinationPoint, m_tweenTime).SetEase(easeType).OnComplete(() =>
            {
                target.DOFade(0, m_fadeTweenTime).OnComplete(() => m_pool.Release(target));
            });
        }

        private void FadeInAndOut(TextMeshProUGUI target)
        {
            target.color = new Color(target.color.r, target.color.g, target.color.b, 0);
            target.DOFade(1, m_fadeTweenTime);
            target.transform.DOLocalMove(m_destinationPoint, m_tweenTime).SetEase(easeType).OnComplete(() =>
            {
                target.DOFade(0, m_fadeTweenTime).OnComplete(() => m_pool.Release(target));
            });
        }
        #endregion
    }
}
