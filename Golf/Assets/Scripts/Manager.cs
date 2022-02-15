using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Manager : MonoBehaviour {

    public GameObject enemy;
    public GameObject projectile;
    public GameObject powerUpProjectile;
    public GameObject spawner;
    public Canvas inGameUI;
    public Canvas mainMenuUI;
    public Slider powerUpSlider;
    public Button playButton;
    public Button upgrade1;
    public Button upgrade2;
    public Button upgrade3;
    public TextMeshProUGUI currencyTextMain;
    public TextMeshProUGUI currencyTextGame;
    public TextMeshProUGUI levelText;


    GameObject[] enemies;
    GameObject[] powerUpProjectiles;

    [HideInInspector]
    public int killStreak = 0;
    [HideInInspector]
    public int enemiesKilled = 0;

    private float   powerUpTimer = 0f;
    public int      powerUpReq = 5;

    float enemySpawnTimer = 1f;

    float pot = 0f;

    float fireRateCounter = 0f;
    float fireRate = 0.5f;

    float ballSpeed = 25f;

    public int fireRateLevel = 1;
    public int ballSpeedLevel = 1;
    int upgradeMaxLevel = 20;

    [HideInInspector]
    public int reward;
    int currency;

    bool playGame = false;

    int enemyNumber = 0;
    int enemiesSpawned = 0;

    public int level = 1;

    void Start() {
        powerUpSlider.maxValue = powerUpReq;
        powerUpSlider.value = 0;
        currency = PlayerPrefs.GetInt("Money",0);
        currencyTextMain.text = currency.ToString();
        currencyTextGame.text = currency.ToString();
        levelText.text = "Level: " + level;
        reward = 0;

        calculateDifficulty(level);

        //Debug.Log("Level: " + level);
        //Debug.Log("EnemyNumber: " + enemyNumber);
        //Debug.Log("enemySpawnTimer: " + enemySpawnTimer);

        playButton.onClick.AddListener(Play);
        upgrade1.onClick.AddListener(Upgrade1);
        upgrade2.onClick.AddListener(Upgrade2);

        //PlayerPrefs.DeleteAll();
    }

    void Update() {
        // Handle Fire Rate Upgrade
        fireRate = Mathf.Lerp(0.5f,0.1f,fireRateLevel/upgradeMaxLevel);

        // Handle Ball Speed Upgrade
        ballSpeed = Mathf.Lerp(25f,50f,ballSpeedLevel/upgradeMaxLevel);

        if (playGame) {
            // Spawn Enemies
            enemySpawnTimer -= Time.deltaTime;
            if (enemySpawnTimer <= 0 && enemiesSpawned < enemyNumber) {
                calculateDifficulty(level);
                Instantiate(enemy, new Vector3(-20+Random.Range(-10,10),2,Random.Range(-7,7)), Quaternion.identity);
                enemiesSpawned++;
            }

            if (enemiesKilled == enemyNumber) {
                MainMenu();
                enemiesKilled = 0;
                enemiesSpawned = 0;
                level++;
                //PlayerPrefs.SetInt("Level", level);
                levelText.text = "Level: " + level;
                calculateDifficulty(level);
            }

            // Reward Player
            currency += reward;
            PlayerPrefs.SetInt("Money", currency);
            currencyTextMain.text = currency.ToString();
            currencyTextGame.text = currency.ToString();
            reward = 0;

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
                    fireProjectile(ref fireRateCounter, fireRate);
                    pot = 0;
                    powerUpSlider.value = killStreak;
                }
            }
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
        inGameUI.enabled = false;
        mainMenuUI.enabled = true;
        mainMenuUI.gameObject.SetActive(true);
        playGame = false;
    }

    void Play() {
        mainMenuUI.enabled = false;
        inGameUI.enabled = true;
        inGameUI.gameObject.SetActive(true);
        playGame = true;
    }

    void Upgrade1() {
        fireRateLevel++;
    }

    void Upgrade2() {
        ballSpeedLevel++;
    }

    void calculateDifficulty (int level) {
        enemyNumber = (int)Mathf.Ceil((level + 10) * 1.2f);
        enemySpawnTimer = Mathf.Clamp(1-level/100f, 0.2f,1f);
    }
}