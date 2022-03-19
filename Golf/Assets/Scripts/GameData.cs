using UnityEngine;

[System.Serializable]
public class GameData {
    public int level, money, fireRateLevel, fireRateCost, ballBounceLevel, ballBounceCost, incomeLevel, incomeCost, soundEnabled, hapticsEnabled;
    public int[,,] cosmetics;
    public int currTheme;


    public GameData(Manager GameManager) {
        level = GameManager.Level;
        money = GameManager.Money;
        fireRateLevel = GameManager.FireRateLevel;
        fireRateCost = GameManager.FireRateCost;
        ballBounceLevel = GameManager.BallBounceLevel;
        ballBounceCost = GameManager.BallBounceCost;
        incomeLevel = GameManager.IncomeLevel;
        incomeCost = GameManager.IncomeCost;
        soundEnabled = GameManager.SoundEnabled;
        hapticsEnabled = GameManager.HapticsEnabled;
        cosmetics = GameManager.Cosmetics;
        currTheme = GameManager.CurrTheme;
    }
}
