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
        OnKillsChange?.Invoke(n);
    }

    public event Action<int, int, int> OnUpgradesChange;
    public void UpgradesChange(int n, int level, int cost) {
        OnUpgradesChange?.Invoke(n,level,cost);
    }

    public event Action<int> OnMoneyChange;
    public void MoneyChange(int n) {
        OnMoneyChange?.Invoke(n);
    }

    public event Action<int> OnLevelChange;
    public void LevelChange(int n) {
        OnLevelChange?.Invoke(n);
    }

    public event Action OnSettingsChange;
    public void SettingsChange() {
        OnSettingsChange?.Invoke();
    }

    public event Action<int, int> OnHandleVictory;
    public void HandleVictory(int n, int j) {
        OnHandleVictory?.Invoke(n,j);
    }

    public event Action OnHandleDefeat;
    public void HandleDefeat() {
        OnHandleDefeat?.Invoke();
    }
}
