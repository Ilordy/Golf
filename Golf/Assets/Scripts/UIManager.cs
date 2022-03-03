using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    Manager GameManager;
    GameObject mainMenu, settingsMenu, shopMenu, ingameUI, victoryUI, defeatUI, pauseUI;
    Button playButton, upgradeButton1, upgradeButton2, upgradeButton3, settingsButton, shopButton, soundsButton, hapticsButton, restoreButton, settingsBackButton;
    Button victoryDoubleButton, victorySkipButton, defeatSkipLevelButton, defeatTryAgainButton, pauseButton, resumeButton, mainMenuButton;
    TextMeshProUGUI levelText, moneyCounterText, upgradeLevelText1, upgradeLevelText2, upgradeLevelText3, upgradeCostText1, upgradeCostText2, upgradeCostText3;
    TextMeshProUGUI victoryText, victoryEarnedText, defeatText, progressText;
    Slider powerUpSlider, progressBar;

    Image soundsButtonImage;
    Image hapticsButtonImage;

    Color colorEnabled = new Color32(117,255,131,255);
    Color colorDisabled = new Color32(255,117,131,255);

    Stack<Transform> menuStack = new Stack<Transform>();

    public Stack<Transform> MenuStack {get{return menuStack;}set{menuStack = value;}}

    void Start() {
        GameEvents.current.OnKillsChange += UpdatePowerUpSlider;
        GameEvents.current.OnUpgradesChange += UpdateUpgradeInfo;
        GameEvents.current.OnMoneyChange += UpdateMoney;
        GameEvents.current.OnSettingsChange += UpdateSettings;
        GameEvents.current.OnLevelChange += UpdateLevel;
        GameEvents.current.OnHandleVictory += HandleVictory;
        GameEvents.current.OnHandleDefeat += HandleDefeat;
        GameEvents.current.OnProgressChange += UpdateProgressBar;

        GameManager = GameObject.Find("Manager").GetComponent<Manager>();

        //Cache Main UI Components
        mainMenu = gameObject.transform.GetChild(0).gameObject;
        settingsMenu = gameObject.transform.GetChild(1).gameObject;
        shopMenu = gameObject.transform.GetChild(2).gameObject;
        ingameUI = gameObject.transform.GetChild(6).gameObject;
        victoryUI = gameObject.transform.GetChild(7).gameObject;
        defeatUI = gameObject.transform.GetChild(8).gameObject;
        pauseUI = gameObject.transform.GetChild(9).gameObject;

        //Set default active UI elements
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        shopMenu.SetActive(false);
        ingameUI.SetActive(false);
        victoryUI.SetActive(false);
        defeatUI.SetActive(false);

        //Chache Main Menu UI Components
        playButton = mainMenu.transform.GetChild(1).GetComponent<Button>();
        upgradeButton1 = mainMenu.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<Button>();
        upgradeButton2 = mainMenu.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<Button>();
        upgradeButton3 = mainMenu.transform.GetChild(2).gameObject.transform.GetChild(2).GetComponent<Button>();
        settingsButton = mainMenu.transform.GetChild(3).GetComponent<Button>();
        shopButton = mainMenu.transform.GetChild(4).GetComponent<Button>();
        levelText = mainMenu.transform.GetChild(1).gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        moneyCounterText = mainMenu.transform.GetChild(5).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        upgradeLevelText1 = mainMenu.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        upgradeCostText1 = mainMenu.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        upgradeLevelText2 = mainMenu.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        upgradeCostText2 = mainMenu.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        upgradeLevelText3 = mainMenu.transform.GetChild(2).gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        upgradeCostText3 = mainMenu.transform.GetChild(2).gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        //Cache Settings UI Components
        soundsButton = settingsMenu.transform.GetChild(1).GetComponent<Button>();
        soundsButtonImage = soundsButton.GetComponent<Image>();
        hapticsButton = settingsMenu.transform.GetChild(2).GetComponent<Button>();
        hapticsButtonImage = hapticsButton.GetComponent<Image>();
        restoreButton = settingsMenu.transform.GetChild(3).GetComponent<Button>();
        settingsBackButton = settingsMenu.transform.GetChild(4).GetComponent<Button>();

        //Cache Shop UI Components

        //Cache ingame UI Components
        powerUpSlider = ingameUI.transform.GetChild(0).GetComponent<Slider>();
        progressBar = ingameUI.transform.GetChild(1).GetComponent<Slider>();
        progressText = ingameUI.transform.GetChild(1).transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        pauseButton = ingameUI.transform.GetChild(2).GetComponent<Button>();

        //Cache Victory/Defeat UI Components
        victoryDoubleButton = victoryUI.transform.GetChild(3).GetComponent<Button>();
        victorySkipButton = victoryUI.transform.GetChild(4).GetComponent<Button>();
        victoryText = victoryUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        victoryEarnedText = victoryUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        defeatSkipLevelButton = defeatUI.transform.GetChild(2).GetComponent<Button>();
        defeatTryAgainButton = defeatUI.transform.GetChild(3).GetComponent<Button>();
        defeatText = defeatUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        //Cache Pause UI Components
        resumeButton = pauseUI.transform.GetChild(3).GetComponent<Button>();
        mainMenuButton = pauseUI.transform.GetChild(4).GetComponent<Button>();

        //Add Listeners for Main Menu Buttons
        playButton.onClick.AddListener(Play);
        upgradeButton1.onClick.AddListener(() => GameEvents.current.Upgrade1Request());
        upgradeButton2.onClick.AddListener(() => GameEvents.current.Upgrade2Request());
        upgradeButton3.onClick.AddListener(() => GameEvents.current.Upgrade3Request());
        settingsButton.onClick.AddListener(OpenSettings);
        shopButton.onClick.AddListener(OpenShop);
        //shopButton.onClick.AddListener(OpenShop);

        //Add Listeners for Settings Menu Buttons
        soundsButton.onClick.AddListener(ToggleSound);
        hapticsButton.onClick.AddListener(ToggleHaptics);
        restoreButton.onClick.AddListener(RestorePurchase);
        settingsBackButton.onClick.AddListener(Back);

        //Add Listeners for Victory/Defeat UI Button
        victoryDoubleButton.onClick.AddListener(AwardDouble);
        victorySkipButton.onClick.AddListener(AwardRegular);
        defeatSkipLevelButton.onClick.AddListener(SkipLevel);
        defeatTryAgainButton.onClick.AddListener(() => OpenMainMenu());

        //Add Listeners for ingame UI
        pauseButton.onClick.AddListener(OpenPauseMenu);

        //Add Listeners for pause UI
        resumeButton.onClick.AddListener(Resume);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        //Add Main Menu as first item in menu stack
        menuStack.Push(mainMenu.transform);

        //Update UI
        UpdateSettings();
    }

    IEnumerator StartGame() {
        yield return new WaitForSeconds(0.1f);
        GameManager.PlayGame = true;
    }

    public void OpenMainMenu() {
        mainMenu.SetActive(true);
        ingameUI.SetActive(false);
        victoryUI.SetActive(false);
        defeatUI.SetActive(false);
        pauseUI.SetActive(false);
    }

    void Play() {
        mainMenu.SetActive(false);
        ingameUI.SetActive(true);
        //GameManager.PlayGame = true;
        StartCoroutine(StartGame());
        menuStack.Push(ingameUI.transform);
    }

    void OpenSettings() {
        DisableInteractable();
        settingsMenu.SetActive(true);
        menuStack.Push(settingsMenu.transform);
    }

    void OpenShop() {
        //DisableInteractable();
        menuStack.Push(shopMenu.transform);
        shopMenu.SetActive(true);
    }

    void ToggleSound() {
        GameManager.SoundEnabled = -GameManager.SoundEnabled;
        UpdateSettings();
    }

    void ToggleHaptics() {
        GameManager.HapticsEnabled = -GameManager.HapticsEnabled;
        UpdateSettings();
    }

    void UpdateSettings() {
        if (GameManager.SoundEnabled > 0) {
            soundsButtonImage.color = colorEnabled;
        } else {
            soundsButtonImage.color = colorDisabled;
        }
        if (GameManager.HapticsEnabled > 0) {
            hapticsButtonImage.color = colorEnabled;
        } else {
            hapticsButtonImage.color = colorDisabled;
        }
    }

    void RestorePurchase() {
        //Restore Purchase for IOS
    }

    void UpdateUpgradeInfo(int upgradeNumber, int upgradeLevel, int upgradeCost) {
        if (upgradeNumber == 1) {
            upgradeLevelText1.text = "Fire Rate " + upgradeLevel.ToString();
            upgradeCostText1.text = upgradeCost.ToString();
        }
        if (upgradeNumber == 2) {
            upgradeLevelText2.text = "Ball Bounce " + upgradeLevel.ToString();
            upgradeCostText2.text = upgradeCost.ToString();
        }
        if (upgradeNumber == 3) {
            upgradeLevelText3.text = "Income " + upgradeLevel.ToString();
            upgradeCostText3.text = upgradeCost.ToString();
        }
    }

    void UpdateMoney(int money) {
        if (money > 99999) {
            moneyCounterText.text = "99999";
        } else {
            moneyCounterText.text = money.ToString();
        }
    }

    void UpdateLevel(int level) {
        levelText.text = "Level: " + level.ToString();
    }

    void UpdatePowerUpSlider(float value) {
        powerUpSlider.value = value;
    }

    void UpdateProgressBar(int value) {
        float progress = (float)value/GameManager.EnemiesToSpawn;
        progressBar.value = progress;
        progressText.text = (progress * 100).ToString("F2") + "%"; 
    }

    void ResetProgressBar() {
        progressBar.value = 0;
        progressText.text = "0%";
    }

    void HandleVictory(int level, int earned) {
        UpdateLevel(level);
        ResetProgressBar();
        victoryEarnedText.text = "You earned " + earned.ToString() + " coins";
        victoryDoubleButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Claim 2x\n" + (earned*2).ToString();
        victorySkipButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Get " + earned.ToString();
        ingameUI.SetActive(false);
        victoryUI.SetActive(true);
    }

    void AwardRegular() {
        GameEvents.current.AwardRegularPressed();
        UpdateMoney(GameManager.Money);
        OpenMainMenu();
    }

    void AwardDouble() {
        GameEvents.current.AwardDoublePressed();
        UpdateMoney(GameManager.Money);
        OpenMainMenu();
    }

    void HandleDefeat() {
        ResetProgressBar();
        ingameUI.SetActive(false);
        defeatUI.SetActive(true);
    }

    void SkipLevel() {
        GameEvents.current.SkipLevelPressed();
        OpenMainMenu();
    }

    void OpenPauseMenu() {
        ingameUI.SetActive(false);
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void Resume() {
        pauseUI.SetActive(false);
        ingameUI.SetActive(true);
        Time.timeScale = 1f;
    }

    void ReturnToMainMenu() {
        ResetProgressBar();
        GameEvents.current.ReturnMainMenu();
        OpenMainMenu();
        Time.timeScale = 1f;
    }

    void DisableInteractable() {
        shopButton.interactable = false;
        settingsButton.interactable = false;
        playButton.interactable = false;
        upgradeButton1.interactable = false;
        upgradeButton2.interactable = false;
        upgradeButton3.interactable = false;
    }

    void EnableInteractable() {
        shopButton.interactable = true;
        settingsButton.interactable = true;
        playButton.interactable = true;
        upgradeButton1.interactable = true;
        upgradeButton2.interactable = true;
        upgradeButton3.interactable = true;
    }

    void Back() {
        menuStack.Peek().gameObject.SetActive(false);
        menuStack.Pop();
        menuStack.Peek().gameObject.SetActive(true);
        if (menuStack.Peek().gameObject == mainMenu) {
            EnableInteractable();
        }
    }
}