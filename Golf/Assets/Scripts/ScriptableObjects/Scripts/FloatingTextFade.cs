using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace MobileTools
{
    [CreateAssetMenu(menuName = "FloatingTextTweeners/AlphaFade")]
    public class FloatingTextFade : FloatingTextTweener
    {
        #region Variables
        private enum FadeType { OnSpawn, OnExit, Both }
        [SerializeField] float m_fadeTweenTime = 1;
        [SerializeField] FadeType m_fadeType;
        private TextMeshProUGUI m_currentText;
        private Vector2 m_destinationPoint;
        #endregion

        #region Methods
        public override void TweenText(TextMeshProUGUI target, Vector2 endPos)
        {
            m_currentText = target;
            m_destinationPoint = endPos;
            DOTween.Kill(m_currentText);
            switch (m_fadeType)
            {
                case FadeType.OnSpawn:
                    FadeIn();
                    break;
                case FadeType.OnExit:
                    FadeOut(target);
                    break;
                case FadeType.Both:
                    FadeInAndOut();
                    break;
            }
        }

        private void FadeIn()
        {
            m_currentText.color = new Color(m_currentText.color.r, m_currentText.color.g, m_currentText.color.b, 0);
            m_currentText.DOFade(1, m_fadeTweenTime);
            m_currentText.transform.DOLocalMove(m_destinationPoint, m_tweenTime);
        }

        private void FadeOut(TextMeshProUGUI text)
        {
            text.transform.DOLocalMove(m_destinationPoint, m_tweenTime).OnComplete(() =>
            {
                text.DOFade(0, m_fadeTweenTime).OnComplete(() => text.GetComponent<PooledText>().Release());
            });
        }

        private void FadeInAndOut()
        {
            m_currentText.color = new Color(m_currentText.color.r, m_currentText.color.g, m_currentText.color.b, 0);
            m_currentText.DOFade(1, m_fadeTweenTime);
            m_currentText.transform.DOLocalMove(m_destinationPoint, m_tweenTime).OnComplete(() =>
            {
                m_currentText.DOFade(0, m_fadeTweenTime)/* .OnComplete(() => m_pool.Release(m_currentText)) */;
            });
        }
        #endregion
    }
}
