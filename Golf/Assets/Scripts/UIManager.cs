using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    Manager GameManager;
    GameObject mainMenu, settingsMenu, shopMenu, ingameUI;
    Button playButton, upgradeButton1, upgradeButton2, upgradeButton3, settingsButton, shopButton, soundsButton, hapticsButton, restoreButton, settingsBackButton;
    TextMeshProUGUI levelText, moneyCounterText, upgradeLevelText1, upgradeLevelText2, upgradeLevelText3, upgradeCostText1, upgradeCostText2, upgradeCostText3;
    Slider powerUpSlider;

    Image soundsButtonImage;
    Image hapticsButtonImage;

    Color colorEnabled = new Color32(117,255,131,255);
    Color colorDisabled = new Color32(255,117,131,255);

    Stack<Transform> menuStack = new Stack<Transform>();

    void Start() {
        GameManager = GameObject.Find("Manager").GetComponent<Manager>();

        //Cache Main UI Components
        mainMenu = gameObject.transform.GetChild(0).gameObject;
        settingsMenu = gameObject.transform.GetChild(1).gameObject;
        shopMenu = gameObject.transform.GetChild(2).gameObject;
        ingameUI = gameObject.transform.GetChild(3).gameObject;

        //Set default active UI elements
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        shopMenu.SetActive(false);
        ingameUI.SetActive(false);

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

        //Add Listeners for Main Menu Buttons
        playButton.onClick.AddListener(Play);
        upgradeButton1.onClick.AddListener(Upgrade1);
        upgradeButton2.onClick.AddListener(Upgrade2);
        upgradeButton3.onClick.AddListener(Upgrade3);
        settingsButton.onClick.AddListener(OpenSettings);
        //shopButton.onClick.AddListener(OpenShop);

        //Add Listeners for Settings Menu Buttons
        soundsButton.onClick.AddListener(ToggleSound);
        hapticsButton.onClick.AddListener(ToggleHaptics);
        restoreButton.onClick.AddListener(RestorePurchase);
        settingsBackButton.onClick.AddListener(Back);

        //Add Main Menu as first item in menu stack
        menuStack.Push(mainMenu.transform);

        //Update UI
        UpdateSettings();
    }

    void Play() {
        mainMenu.SetActive(false);
        ingameUI.SetActive(true);
        GameManager.playGame = true;
        menuStack.Push(ingameUI.transform);
    }

    void Upgrade1() {
        GameManager.HandleUpgrade1();
    }

    void Upgrade2() {
        GameManager.HandleUpgrade2();
    }

    void Upgrade3() {
        GameManager.HandleUpgrade3();
    }

    void OpenSettings() {
        settingsMenu.SetActive(true);
        menuStack.Push(settingsMenu.transform);
    }

    void OpenShop() {
        settingsMenu.SetActive(true);
        menuStack.Push(shopMenu.transform);
    }

    void ToggleSound() {
        GameManager.SetSound(-GameManager.GetSound());
        UpdateSettings();
    }

    void ToggleHaptics() {
        GameManager.SetHaptics(-GameManager.GetHaptics());
        UpdateSettings();
    }

    void UpdateSettings() {
        if (GameManager.GetSound() > 0) {
            soundsButtonImage.color = colorEnabled;
        } else {
            soundsButtonImage.color = colorDisabled;
        }
        if (GameManager.GetHaptics() > 0) {
            hapticsButtonImage.color = colorEnabled;
        } else {
            hapticsButtonImage.color = colorDisabled;
        }
    }

    void RestorePurchase() {
        //Restore Purchase for IOS
    }

    public void UpdateUpgradeInfo(int upgradeNumber, int upgradeLevel, int upgradeCost) {
        if (upgradeNumber == 1) {
            upgradeLevelText1.text = upgradeLevel.ToString();
            upgradeCostText1.text = upgradeCost.ToString();
        }
        if (upgradeNumber == 2) {
            upgradeLevelText2.text = upgradeLevel.ToString();
            upgradeCostText2.text = upgradeCost.ToString();
        }
        if (upgradeNumber == 3) {
            upgradeLevelText3.text = upgradeLevel.ToString();
            upgradeCostText3.text = upgradeCost.ToString();
        }
    }

    public void UpdateMoney(int money) {
        moneyCounterText.text = money.ToString();
    }

    public void UpdatePowerUpSlider(float value) {
        powerUpSlider.value = value;
    }

    public void OpenMainMenu() {
        mainMenu.SetActive(true);
        ingameUI.SetActive(false);
    }

    public void HandleVictory() {

    }

    public void HandleDefeat() {

    }

    void Back() {
        menuStack.Peek().gameObject.SetActive(false);
        menuStack.Pop();
        menuStack.Peek().gameObject.SetActive(true);
    }
}