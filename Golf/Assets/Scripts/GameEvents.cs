using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    
    public static GameEvents current;

    void Awake()
    {
        current = this;
    }

    public event Action<float> OnKillsChange;
    public void KillsChange(float n) {
        if (OnKillsChange != null) {
            OnKillsChange(n);
        }
    }

    public event Action<int, int, int> OnUpgradesChange;
    public void UpgradesChange(int n, int level, int cost) {
        if (OnUpgradesChange != null) {
            OnUpgradesChange(n, level, cost);
        }
    }

    public event Action<int> OnMoneyChange;
    public void MoneyChange(int n) {
        if (OnMoneyChange != null) {
            OnMoneyChange(n);
        }
    }

    public event Action<int> OnLevelChange;
    public void LevelChange(int n) {
        if (OnLevelChange != null) {
            OnLevelChange(n);
        }
    }

    public event Action OnSettingsChange;
    public void SettingsChange() {
        if (OnSettingsChange != null) {
            OnSettingsChange();
        }
    }

    public event Action<int, int> OnHandleVictory;
    public void HandleVictory(int n, int j) {
        if (OnHandleVictory != null) {
            OnHandleVictory(n, j);
        }
    }

    public event Action OnHandleDefeat;
    public void HandleDefeat() {
        if (OnHandleDefeat != null) {
            OnHandleDefeat();
        }
    }
}
