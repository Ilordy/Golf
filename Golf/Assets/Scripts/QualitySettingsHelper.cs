using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

namespace MobileTools
{
    /// <summary>
    /// 
    /// </summary>
    public class QualitySettingsHelper : MonoBehaviour
    {
        #region Variables
        [SerializeField, Tooltip("Should the fpsDisplay be instantiated during builds?")]
        bool m_displayFPSInBuilds = true;

        [SerializeField, Tooltip("Should this component destroy itself upon entering the lowest quality level?")]
        bool m_DestroyAtLastLevel;

        [SerializeField, Tooltip("object to reference during runtime to displayFPS. (Can be left null)")]
        TextMeshProUGUI m_fpsDisplayer;

        [SerializeField, Tooltip("Default frame rate for targetted platform. (Left on mobile frame rate)")]
        int m_defaultFrameRate = 30;

        [SerializeField, Tooltip("How much of a difference can the frame rate drop relative to it's target frame rate?")]
        int m_differenceThreshold = 5;

        [SerializeField, Tooltip("How often in seconds should the application's framerate be checked before making a decision?")]
        float m_fpsCheckInterval = 3;

        [SerializeField] TextMeshProUGUI applicationFPS; //for testing
        [SerializeField, ReadOnly] float m_currentFPS;
        private float m_count;
        #endregion

        #region LifeCycle
        private void Start()
        {
#if !UNITY_EDITOR
            if (!m_displayFPSInBuilds && m_fpsDisplayer != null)
                Destroy(m_fpsDisplayer.gameObject);
#endif
            //StartCoroutine(CheckFrameRate());
            //Setting to final quality level.
            QualitySettings.SetQualityLevel(QualitySettings.names.Length, false);
            applicationFPS.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
        }

        void Update()
        {
            m_count = 1f / Time.unscaledDeltaTime;
            //Debug.Log(Application.targetFrameRate + " THIS THE FRAMERATE");
            if (m_fpsDisplayer == null) return;
            m_fpsDisplayer.text = "FPS: " + Mathf.Round(m_count);
        }
        #endregion

        #region Private Methods
        IEnumerator CheckFrameRate()
        {
            yield return new WaitForSecondsRealtime(m_fpsCheckInterval);
            if (m_defaultFrameRate - m_count > m_differenceThreshold)
            {
                QualitySettings.DecreaseLevel(false);
                if (QualitySettings.GetQualityLevel() == 0 && m_DestroyAtLastLevel) //if we're at the last quality level.
                    Destroy(this);
            }

            StartCoroutine(CheckFrameRate());
        }
        #endregion
    }
}