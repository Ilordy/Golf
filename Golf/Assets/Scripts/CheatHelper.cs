using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatHelper : MonoBehaviour
{
    #region Variables
    [SerializeField] TMP_InputField m_levelInputField;
    [SerializeField] Button m_levelConfirmButton;
    [SerializeField] AnimationCurve m_curve;
    [SerializeField] float m_testCurve;
    #endregion

    #region Life Cycle
    void Start()
    {
        m_levelConfirmButton.onClick.AddListener(SetLevel);
    }

    void Update()
    {
        Debug.Log(m_curve.Evaluate(m_testCurve));
        Debug.Log(Mathf.Log(10000,10));
    }
    #endregion

    #region Methods
    void SetLevel()
    {
        if (int.TryParse(m_levelInputField.text, out int result))
        {
            Manager.I.SetLevel(result);
        }
        else
            Debug.LogError("Unable to parse Int from Cheat Level input field!");
    }
    #endregion
}
