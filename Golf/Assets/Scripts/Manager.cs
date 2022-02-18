using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;


public class Manager : MonoBehaviour {

    public GameObject player;
    public GameObject enemy;
    public GameObject projectile;
    public GameObject powerUpProjectile;
    public GameObject spawner;
    public Canvas inGameUI;
    public Canvas mainMenuUI;
    public Canvas settingsUI;
    public Canvas shopUI;
    Transform shopMainUI, hatsUI, skinsUI, trailsUI, ballSkinsUI;

    Stack<Transform> menuStack = new Stack<Transform>();

    Slider powerUpSlider;
    Button playButton, upgrade1, upgrade2, upgrade3, settings, sounds, haptics, settingsBack, shop, shopBack,
    shopHats, shopSkins, shopTrails, shopBallSkins, hatsUIBack, skinsUIBack, trailsUIBack, ballSkinsUIBack;
    TextMeshProUGUI currencyTextMain, levelText, currencyTextGame;
    Animator playerAnimator;

    Color colorEnabled = new Color32(117,255,131,255);
    Color colorDisabled = new Color32(255,117,131,255);

    int soundEnabled = 1;
    int hapticsEnabled = 1;

    GameObject[] enemies;
    GameObject[] powerUpProjectiles;

    [HideInInspector]
    public int killStreak = 0;
    [HideInInspector]
    public int enemiesKilled = 0;
    [HideInInspector]
    public bool reward = false;

    float   powerUpTimer = 0f;
    int      powerUpReq = 5;

    float enemySpawnTimer = 1f;

    float pot = 0f;

    float fireRateCounter = 0f;

    float fireRate = 0.5f;
    float ballSpeed = 25f;
    int income = 1;

    int fireRateLevel = 1;
    int fireRateCost = 100;

    int ballSpeedLevel = 1;
    int ballSpeedCost = 100;

    int incomeLevel = 1;
    int incomeCost = 100;

    int upgradeMaxLevel = 20;

    int currency;

    bool playGame = false;

    int enemyNumber = 0;
    int enemiesSpawned = 0;

    int level = 1;

    void Start() {
        mainMenuUI.gameObject.SetActive(true);
        inGameUI.gameObject.SetActive(false);
        settingsUI.gameObject.SetActive(false);
        shopUI.gameObject.SetActive(false);

        menuStack.Push(mainMenuUI.transform);

        playerAnimator = player.GetComponent<Animator>();
        powerUpSlider = inGameUI.transform.GetChild(0).GetComponent<Slider>();
        playButton = mainMenuUI.transform.GetChild(3).GetComponent<Button>();
        upgrade1 = mainMenuUI.transform.GetChild(4).gameObject.transform.GetChild(0).GetComponent<Button>();
        upgrade2 = mainMenuUI.transform.GetChild(4).transform.GetChild(1).GetComponent<Button>();
        upgrade3 = mainMenuUI.transform.GetChild(4).transform.GetChild(2).GetComponent<Button>();
        settings = mainMenuUI.transform.GetChild(5).GetComponent<Button>();
        currencyTextMain = mainMenuUI.transform.GetChild(7).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        levelText = mainMenuUI.transform.GetChild(3).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        currencyTextGame = inGameUI.transform.GetChild(1).transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        shop = mainMenuUI.transform.GetChild(6).GetComponent<Button>();
        sounds = settingsUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>();
        haptics = settingsUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Button>();
        settingsBack = settingsUI.transform.GetChild(0).transform.GetChild(4).GetComponent<Button>();
        shopBack = shopUI.transform.GetChild(1).transform.GetChild(5).GetComponent<Button>();
        shopHats = shopUI.transform.GetChild(1).transform.GetChild(1).GetComponent<Button>();
        shopSkins = shopUI.transform.GetChild(1).transform.GetChild(2).GetComponent<Button>();
        shopTrails = shopUI.transform.GetChild(1).transform.GetChild(3).GetComponent<Button>();
        shopBallSkins = shopUI.transform.GetChild(1).transform.GetChild(4).GetComponent<Button>();
        shopMainUI = shopUI.transform.GetChild(1).transform;
        hatsUI = shopUI.transform.GetChild(2).transform;
        hatsUIBack = hatsUI.GetChild(hatsUI.childCount-1).GetComponent<Button>();
        skinsUI = shopUI.transform.GetChild(3).transform;
        skinsUIBack = skinsUI.GetChild(skinsUI.childCount-1).GetComponent<Button>();
        trailsUI = shopUI.transform.GetChild(4).transform;
        trailsUIBack = trailsUI.GetChild(trailsUI.childCount-1).GetComponent<Button>();
        ballSkinsUI = shopUI.transform.GetChild(5).transform;
        ballSkinsUIBack = ballSkinsUI.GetChild(ballSkinsUI.childCount-1).GetComponent<Button>();

        playButton.onClick.AddListener(Play);
        upgrade1.onClick.AddListener(Upgrade1);
        upgrade2.onClick.AddListener(Upgrade2);
        upgrade3.onClick.AddListener(Upgrade3);
        settings.onClick.AddListener(Settings);
        sounds.onClick.AddListener(Sounds);
        haptics.onClick.AddListener(Haptics);
        shop.onClick.AddListener(Shop);
        settingsBack.onClick.AddListener(Back);
        shopBack.onClick.AddListener(Back);
        hatsUIBack.onClick.AddListener(Back);
        skinsUIBack.onClick.AddListener(Back);
        trailsUIBack.onClick.AddListener(Back);
        ballSkinsUIBack.onClick.AddListener(Back);
        shopHats.onClick.AddListener(ShopHats);
        shopSkins.onClick.AddListener(ShopSkins);
        shopTrails.onClick.AddListener(ShopTrails);
        shopBallSkins.onClick.AddListener(ShopBallSkins);


        powerUpSlider.maxValue = powerUpReq;
        powerUpSlider.value = 0;
        level = PlayerPrefs.GetInt("Level", 1);
        currency = PlayerPrefs.GetInt("Money",4000000);
        fireRateLevel = PlayerPrefs.GetInt("FireRateLevel", 1);
        fireRateCost = PlayerPrefs.GetInt("FireRateCost", 100);
        ballSpeedLevel = PlayerPrefs.GetInt("BallSpeedLevel", 1);
        ballSpeedCost = PlayerPrefs.GetInt("BallSpeedCost", 100);
        incomeLevel = PlayerPrefs.GetInt("IncomeLevel", 1);
        incomeCost = PlayerPrefs.GetInt("IncomeCost", 100);
        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1);
        hapticsEnabled = PlayerPrefs.GetInt("HapticsEnabled", 1);

        currencyTextMain.text = currency.ToString();
        currencyTextGame.text = currency.ToString();
        levelText.text = "Level: " + level;
        //sounds.GetComponent<Image>().color = colorEnabled;
        //haptics.GetComponent<Image>().color = colorDisabled;

        calculateDifficulty(level);

        //Debug.Log("Level: " + level);
        //Debug.Log("EnemyNumber: " + enemyNumber);
        //Debug.Log("enemySpawnTimer: " + enemySpawnTimer);

        

        //PlayerPrefs.DeleteAll();
    }

    void Update() {
        // Update UI
        currencyTextMain.text = currency.ToString();
        currencyTextGame.text = currency.ToString();

        upgrade1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Fire Rate " + fireRateLevel.ToString();
        upgrade1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = fireRateCost.ToString();
        upgrade2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Ball Speed " + ballSpeedLevel.ToString();
        upgrade2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ballSpeedCost.ToString();
        upgrade3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Income " + incomeLevel.ToString();
        upgrade3.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = incomeCost.ToString();

        if (hapticsEnabled > 0) {
            haptics.GetComponent<Image>().color = colorEnabled;
        } else {
            haptics.GetComponent<Image>().color = colorDisabled;
        }

        if (soundEnabled > 0) {
            sounds.GetComponent<Image>().color = colorEnabled;
        } else {
            sounds.GetComponent<Image>().color = colorDisabled;
        }


        // Handle Fire Rate Upgrade
        //fireRate = Mathf.Lerp(0.5f,0.1f,fireRateLevel/upgradeMaxLevel);
        fireRate = Mathf.Lerp(1.0f,5.0f,(float)fireRateLevel/upgradeMaxLevel);
        playerAnimator.SetFloat("Speed", fireRate);

        // Handle Ball Speed Upgrade
        ballSpeed = Mathf.Lerp(25f,50f,ballSpeedLevel/upgradeMaxLevel);

        // Handle Income Upgrade
        income = incomeLevel;

        //Gameplay
        if (playGame) {
            // Spawn Enemies
            enemySpawnTimer -= Time.deltaTime;
            if (enemySpawnTimer <= 0 && enemiesSpawned < enemyNumber) {
                calculateDifficulty(level);
                Instantiate(enemy, new Vector3(-30-Random.Range(0,10),2,Random.Range(-4.7f,4.7f)), Quaternion.identity);
                enemiesSpawned++;
            }

            if (enemiesKilled == enemyNumber) {
                MainMenu();
                enemiesKilled = 0;
                enemiesSpawned = 0;
                level++;
                PlayerPrefs.SetInt("Level", level);
                levelText.text = "Level: " + level;
                calculateDifficulty(level);
            }

            // Reward Player
            if (reward) {
                currency += Random.Range(1, income);
                PlayerPrefs.SetInt("Money", currency);
                reward = false;
            }

            // Adjust Power Up Slider
            if (killStreak <= powerUpReq) {
                powerUpSlider.value = killStreak;
            }

            // Shooting / Power up
            if (Input.GetMouseButtonDown(0)) {
                powerUpTimer = Time.time;
            }

            if (Input.GetMouseButton(0)) {
                float holdTime = Time.time - powerUpTimer;
                if (killStreak >= powerUpReq && holdTime > 0.1f) {
                    pot += Time.deltaTime;
                    powerUpSlider.value = Mathf.Lerp(powerUpReq, 0, pot / 1f);
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                float holdTime = Time.time - powerUpTimer;
                if (holdTime >= 1f && killStreak >= powerUpReq) {
                    enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    powerUpProjectiles = new GameObject[enemies.Length];
                    for (int i = 0; i <= enemies.Length - 1; i++) {
                        int r = 3;
                        float x = r * Mathf.Cos(i * (Mathf.PI / (enemies.Length - 1)));
                        float y = r * Mathf.Sin(i * (Mathf.PI / (enemies.Length - 1)));
                        powerUpProjectiles[i] = Instantiate(powerUpProjectile, spawner.transform.position + new Vector3(0, y, x), Quaternion.identity);
                        powerUpProjectiles[i].GetComponent<Rigidbody>().useGravity = true;
                        powerUpProjectiles[i].transform.LookAt(enemies[i].transform.position);
                        powerUpProjectiles[i].GetComponent<Rigidbody>().velocity = powerUpProjectiles[i].transform.forward * 100f;
                    }
                    killStreak = 0;
                    pot = 0;
                } else {
                    //fireProjectile(ref fireRateCounter, fireRate);
                    playerAnimator.SetBool("Swing",false);
                    playerAnimator.SetBool("Swing",true);
                    pot = 0;
                    powerUpSlider.value = killStreak;
                }
            }
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            playerAnimator.SetBool("Swing",false);
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f) {
            fireProjectile(ref fireRateCounter, Mathf.Lerp(0.8f,0.4f, (float)fireRateLevel/upgradeMaxLevel));
            //Debug.Log("Fire Rate Level: " + fireRateLevel);
            //Debug.Log("Fire Rate: " + fireRate);
            //Debug.Log("Speed Multiplier: " + playerAnimator.GetFloat("Speed"));
        }
    }

    // Functions
    void fireProjectile(ref float fireRateCounter, float nextFire) {
        if (Time.time > fireRateCounter) {
            fireRateCounter = Time.time + nextFire;
            Vector3 flatAimTarget = calculateTarget();
            GameObject p = Instantiate(projectile, spawner.transform.position, Quaternion.identity);
            p.transform.LookAt(flatAimTarget);
            p.GetComponent<Rigidbody>().useGravity = true;
            p.GetComponent<Rigidbody>().velocity = p.transform.forward * ballSpeed;
        }
    }

    Vector3 calculateTarget() {
        Vector3 cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        Vector3 screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return screenPoint + cursorRay / Mathf.Abs(cursorRay.y) * Mathf.Abs(screenPoint.y - spawner.transform.position.y);
    }

    void MainMenu() {
        inGameUI.gameObject.SetActive(false);
        shopUI.gameObject.SetActive(false);
        settingsUI.gameObject.SetActive(false);
        mainMenuUI.gameObject.SetActive(true);
        playGame = false;
    }

    void Play() {
        mainMenuUI.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(true);
        playGame = true;
    }

    void Settings() {
        //mainMenuUI.gameObject.SetActive(false);
        settingsUI.gameObject.SetActive(true);
        menuStack.Push(settingsUI.transform);
    }

    void Sounds() {
        soundEnabled = -soundEnabled;
        PlayerPrefs.SetInt("SoundEnabled", soundEnabled);
    }

    void Haptics() {
        hapticsEnabled = -hapticsEnabled;
        PlayerPrefs.SetInt("HapticsEnabled", hapticsEnabled);
    }

    void Shop() {
        //mainMenuUI.gameObject.SetActive(false);
        shopUI.gameObject.SetActive(true);
        shopMainUI.gameObject.SetActive(true);
        menuStack.Push(shopUI.transform);
    }

    void ShopHats() {
        shopMainUI.gameObject.SetActive(false);
        hatsUI.gameObject.SetActive(true);
        menuStack.Push(hatsUI);
    }

    void ShopSkins() {
        shopMainUI.gameObject.SetActive(false);
        skinsUI.gameObject.SetActive(true);
        menuStack.Push(skinsUI);
    }

    void ShopTrails() {
        shopMainUI.gameObject.SetActive(false);
        trailsUI.gameObject.SetActive(true);
        menuStack.Push(trailsUI);
    }

    void ShopBallSkins() {
        shopMainUI.gameObject.SetActive(false);
        ballSkinsUI.gameObject.SetActive(true);
        menuStack.Push(ballSkinsUI);
    }

    void Back() {
        menuStack.Peek().gameObject.SetActive(false);
        menuStack.Pop();
        menuStack.Peek().gameObject.SetActive(true);
        shopMainUI.gameObject.SetActive(true);
        //shopUI.gameObject.SetActive(false);
        //settingsUI.gameObject.SetActive(false);
    }

    void Upgrade1() {
        if (currency >= fireRateCost && fireRateLevel < upgradeMaxLevel) {
            currency -= fireRateCost;
            fireRateLevel++;
            fireRateCost = 100 * fireRateLevel;
            //upgrade1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Fire Rate " + fireRateLevel.ToString();
            //upgrade1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = fireRateCost.ToString();
            PlayerPrefs.SetInt("FireRateLevel", fireRateLevel);
            PlayerPrefs.SetInt("FireRateCost", fireRateCost);
            
        }
        else {
            //upgrade1.interactable = false;
        }
    }

    void Upgrade2() {
        if (currency >= ballSpeedCost && ballSpeedLevel < upgradeMaxLevel) {
            currency -= ballSpeedCost;
            ballSpeedLevel++;
            ballSpeedCost = 100 * ballSpeedLevel;
            //upgrade2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Ball Speed " + ballSpeedLevel.ToString();
            //upgrade2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ballSpeedCost.ToString();
            PlayerPrefs.SetInt("BallSpeedLevel", ballSpeedLevel);
            PlayerPrefs.SetInt("BallSpeedCost", ballSpeedCost);
            
        } else {
            //upgrade2.interactable = false;
        }
    }

    void Upgrade3() {
        if (currency >= incomeCost && incomeLevel < upgradeMaxLevel) {
            currency -= incomeCost;
            incomeLevel++;
            incomeCost = 100 * incomeLevel;
            //upgrade3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Income " + ballSpeedLevel.ToString();
            //upgrade3.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = incomeCost.ToString();
            PlayerPrefs.SetInt("IncomeLevel", ballSpeedLevel);
            PlayerPrefs.SetInt("IncomeCost", ballSpeedCost);
            
        } else {
            //upgrade2.interactable = false;
        }
    }

    void calculateDifficulty (int level) {
        enemyNumber = (int)Mathf.Ceil((level + 10) * 1.2f);
        enemySpawnTimer = Mathf.Clamp(1-level/100f, 0.2f,1f);
    }
}