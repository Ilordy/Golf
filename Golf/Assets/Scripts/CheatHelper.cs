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
    #endregion

    #region Life Cycle
    void Start()
    {
        m_levelConfirmButton.onClick.AddListener(SetLevel);
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
