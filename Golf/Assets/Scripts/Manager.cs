using UnityEngine;
using System;
using System.Collections;


namespace UnityEngine {

public class Manager : MonoBehaviour {

    [SerializeField] GameObject player;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject cartEnemy;
    [SerializeField] GameObject shieldEnemy;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject powerUpProjectile;
    [SerializeField] GameObject spawner;

    EnemyClass EnemyClass;
    Animator playerAnimator;

    //Settings
    int soundEnabled = 1;
    int hapticsEnabled = 1;

    //Power Up
    GameObject[] enemies;
    GameObject[] powerUpProjectiles;

    //Shooting
    float   beganHolding = 0f;
    float   pot = 0f;
    float   fireRateCounter = 0f;
    int     powerUpReq = 5;

    //Enemy Spawning
    IEnumerator StartedSpawning;
    int     enemiesToSpawn = 0;
    float   enemySpawnInterval = 1f;

    //Upgrades
    float fireRate = 0.5f;
    int maxBounces = 4;
    int income = 1;
    int fireRateLevel = 1;
    int fireRateCost = 100;
    int ballBounceLevel = 1;
    int ballBounceCost = 1000;
    int incomeLevel = 1;
    int incomeCost = 100;
    int upgradeMaxLevel = 20;
    int money = 0;
    int earned = 0;

    //Cosmetics
    int[,,] cosmetics = new int[3,7,2];
    Gradient currTrail;
    [SerializeField] Material defaultMat;

    //Game loop
    bool playGame = false;
    bool themeSet = false;
    int currTheme;
    int level = 1;

    //Getters/Setters
    public bool PlayGame {set{playGame = value;}}
    public int SoundEnabled {get{return soundEnabled;}set{soundEnabled = value;}}
    public int HapticsEnabled {get{return hapticsEnabled;}set{hapticsEnabled = value;}}
    public int MaxBounces {get{return maxBounces;}}
    public int EnemiesToSpawn {get{return enemiesToSpawn;}}
    public int Money {get => money;}
    public int Level {get => level;}
    public int FireRateLevel {get => fireRateLevel;}
    public int FireRateCost {get => fireRateCost;}
    public int BallBounceLevel {get => ballBounceLevel;}
    public int BallBounceCost {get => ballBounceCost;}
    public int IncomeLevel {get => incomeLevel;}
    public int IncomeCost {get => incomeCost;}
    public int[,,] Cosmetics {get => cosmetics;}
    public int CurrTheme {get => currTheme;}


    void Start() {
        Handheld.Vibrate();
        //Subscribe to events
        GameEvents.current.OnUpgrade1Request += HandleUpgrade1;
        GameEvents.current.OnUpgrade2Request += HandleUpgrade2;
        GameEvents.current.OnUpgrade3Request += HandleUpgrade3;
        GameEvents.current.OnCheckAffordUI += CheckMoney;
        GameEvents.current.OnRewardVictory += RewardVictory;
        GameEvents.current.OnSkipLevelPressed += SkipLevel;
        GameEvents.current.OnDefeat += HandleDefeat;
        GameEvents.current.OnReward += HandleReward;
        GameEvents.current.OnReturnMainMenu += ResetGame;
        GameEvents.current.OnRequestCosmetic += HandleCosmetic;
        GameEvents.current.OnSetEquip += HandleCosmeticState;
        GameEvents.current.OnLoadTrail += LoadTrail;
        GameEvents.current.OnSetStartMaterial += SetPlayerMaterial;
        GameEvents.current.OnRequestCosmeticStates += SendCosmeticStates;

        //Get EnemyClass
        EnemyClass = GetComponent<EnemyClass>();
        //Set Player Animator
        playerAnimator = player.GetComponent<Animator>();

        //LOAD GAME DATA
        //DeleteData();
        //LoadData();

        money = 20000;

        //Set player default material
        player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial = defaultMat;

        //Calculate difficulty for current level
        calculateDifficulty(level);
        
        //Update Upgrade Values
        UpdateUpgradeValues();
        CheckMoney();

        //Equip current cosmetics
        for (int i = 0; i < cosmetics.GetLength(0); i++) {
            for (int j = 0; j < cosmetics.GetLength(1); j++) {
                if (cosmetics[i,j,0] == 1 && cosmetics[i,j,1] == 1) {
                    GameEvents.current.EquipCurrent(i,j);
                    break;
                }
            }
        }

        //Update UI
        GameEvents.current.SettingsChange();
        GameEvents.current.LevelChange(level);
        GameEvents.current.MoneyChange();
        GameEvents.current.UpgradesChange(1, fireRateLevel, fireRateCost);
        GameEvents.current.UpgradesChange(2, ballBounceLevel, ballBounceCost);
        GameEvents.current.UpgradesChange(3, incomeLevel, incomeCost);
    }

    void Update() {
        //Gameplay
        if (!playGame) return;

        if (StartedSpawning == null) {
            StartedSpawning = SpawnEnemy();
            StartCoroutine(StartedSpawning);
        } else {
            //HANDLE VICTORY
            if (EnemyClass.TotalKilledCount == EnemyClass.AliveCount) {
                HandleVictory();
            }
        }

        // Shooting / Power up
        if (Input.GetMouseButtonDown(0)) {
            beganHolding = Time.time;
        }

        if (Input.GetMouseButton(0)) {
            float holdTime = Time.time - beganHolding;
            if (EnemyClass.KilledCount >= powerUpReq && holdTime > 0.1f) {
                enemies = GameObject.FindGameObjectsWithTag("Enemy");
                pot += Time.deltaTime;
                GameEvents.current.AnimatePowerUp(enemies.Length, pot);
                GameEvents.current.KillsChange(Mathf.Lerp(powerUpReq, 0, pot / 1f));
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            float holdTime = Time.time - beganHolding;
            if (holdTime >= 1f && EnemyClass.KilledCount >= powerUpReq) {
                powerUpProjectiles = new GameObject[enemies.Length];
                for (int i = 0; i < enemies.Length; i++) {
                    int r = 3;
                    float x = r * Mathf.Cos(i * (Mathf.PI / (enemies.Length)));
                    float y = r * Mathf.Sin(i * (Mathf.PI / (enemies.Length)));
                    powerUpProjectiles[i] = Instantiate(powerUpProjectile, spawner.transform.position + new Vector3(0, y, x), Quaternion.identity);
                    if (currTrail != null) powerUpProjectiles[i].GetComponent<TrailRenderer>().colorGradient = currTrail;
                    powerUpProjectiles[i].GetComponent<Rigidbody>().useGravity = true;
                    powerUpProjectiles[i].transform.LookAt(enemies[i].transform.position);
                    powerUpProjectiles[i].GetComponent<Rigidbody>().velocity = powerUpProjectiles[i].transform.forward * 100f;
                }
                EnemyClass.KilledCount = 0;
                pot = 0;
                GameEvents.current.DeleteAnimation();
                GameEvents.current.PowerUp();
            } else {
                //playerAnimator.SetBool("Swing",false);
                playerAnimator.SetBool("Swing",true);
                pot = 0;
                GameEvents.current.DeleteAnimation();
                GameEvents.current.KillsChange(EnemyClass.KilledCount);
            }
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            playerAnimator.SetBool("Swing",false);
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f) {
            fireProjectile(ref fireRateCounter, Mathf.Lerp(0.8f,0.4f, (float)fireRateLevel/upgradeMaxLevel));
        }
    }

    // TESTING
    void OnApplicationFocus(bool hasFocus) {
        //SaveData();
    }

    // Functions

    void DeleteData() {
        SaveSystem.DeleteSaves();
    }

    void SaveData() {
        SaveSystem.SaveData(this);
    }

    void LoadData() {
        GameData data = SaveSystem.LoadData();

        if (data == null) return;

        level = data.level;
        money = data.money;
        fireRateLevel = data.fireRateLevel;
        fireRateCost = data.fireRateCost;
        ballBounceLevel = data.ballBounceLevel;
        ballBounceCost = data.ballBounceCost;
        incomeLevel = data.incomeLevel;
        incomeCost = data.incomeCost;
        soundEnabled = data.soundEnabled;
        hapticsEnabled = data.hapticsEnabled;
        cosmetics = data.cosmetics;
        currTheme = data.currTheme;
    }

    IEnumerator SpawnEnemy() {
        while (EnemyClass.AliveCount < enemiesToSpawn) {
            float chance = Random.Range(0f,1f);
            float xPos = Random.Range(-47f,-32f);
            float zPos = Random.Range(-3.65f,3.65f);
            GameObject selected = null;
            if (chance < 0.1f && level > 10) {
                selected = cartEnemy;
            } else if (chance > 0.1f && chance < 0.3f && level > 5) {
                selected = shieldEnemy;
            } else {
                selected = enemy;
            }
            Instantiate(selected, new Vector3(xPos,2,zPos), Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
    }

    //FIRE PROJECTILES
    void fireProjectile(ref float fireRateCounter, float nextFire) {
        if (Time.time > fireRateCounter) {
            fireRateCounter = Time.time + nextFire;
            Vector3 flatAimTarget = calculateTarget();
            GameObject p = Instantiate(projectile, spawner.transform.position, Quaternion.identity);
            if (currTrail != null) p.GetComponent<TrailRenderer>().colorGradient = currTrail;
            p.transform.LookAt(flatAimTarget);
            p.GetComponent<Rigidbody>().useGravity = true;
            // p.GetComponent<Rigidbody>().velocity = p.transform.forward * 10f;
            p.GetComponent<Rigidbody>().AddForce(p.transform.forward * 25f, ForceMode.Impulse);
        }
    }

    Vector3 calculateTarget() {
        Vector3 cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        Vector3 screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 target = screenPoint + cursorRay / Mathf.Abs(cursorRay.y) * Mathf.Abs(screenPoint.y - spawner.transform.position.y);
        return new Vector3(target.x, 0.63f, target.z);
    }

    void calculateDifficulty (int level) {
        enemiesToSpawn = Mathf.Clamp((int)Mathf.Ceil((level + 10) * 1.2f), 0,300);
        enemySpawnInterval = Mathf.Clamp(1-level/100f, 0.2f,1f);
    }

    void UpdateUpgradeValues() {
        fireRate = Mathf.Lerp(1.0f,5.0f,(float)fireRateLevel/upgradeMaxLevel);
        maxBounces = ballBounceLevel;
        playerAnimator.SetFloat("Speed", fireRate);
        income = incomeLevel;
    }

    // EVENT FUNCTIONS
    public void HandleUpgrade1() {
        if (money > fireRateCost && fireRateLevel < upgradeMaxLevel) {
            money -= fireRateCost;
            fireRateLevel++;
            fireRateCost += 100;
        }
        CheckMoney();
        UpdateUpgradeValues();
        GameEvents.current.MoneyChange();
        GameEvents.current.UpgradesChange(1, fireRateLevel, fireRateCost);
        PlayerPrefs.SetInt("FireRateLevel", fireRateLevel);
        PlayerPrefs.SetInt("FireRateCost", fireRateCost);
        PlayerPrefs.SetInt("Money", money);
    }

    public void HandleUpgrade2() {
        if (money > ballBounceCost && ballBounceLevel < 5) {
            money -= ballBounceCost;
            ballBounceLevel++;
            ballBounceCost += 1000;
        }
        CheckMoney();
        UpdateUpgradeValues();
        GameEvents.current.MoneyChange();
        GameEvents.current.UpgradesChange(2, ballBounceLevel, ballBounceCost);
        PlayerPrefs.SetInt("BallBounceLevel", ballBounceLevel);
        PlayerPrefs.SetInt("BallBounceCost", ballBounceCost);
        PlayerPrefs.SetInt("Money", money);
    }

    public void HandleUpgrade3() {
        if (money > incomeCost && incomeLevel < upgradeMaxLevel) {
            money -= incomeCost;
            incomeLevel++;
            incomeCost += 100;
        }
        CheckMoney();
        UpdateUpgradeValues();
        GameEvents.current.MoneyChange();
        GameEvents.current.UpgradesChange(3, incomeLevel, incomeCost);
        PlayerPrefs.SetInt("IncomeLevel", incomeLevel);
        PlayerPrefs.SetInt("IncomeCost", incomeCost);
        PlayerPrefs.SetInt("Money", money);
    }

    void HandleVictory() {
        themeSet = false;
        ResetGame();
        level++;
        PlayerPrefs.SetInt("Level", level);
        calculateDifficulty(level);
        GameEvents.current.HandleVictory(level, earned);
    }

    void RewardVictory(int mult) {
        HandleTheme();
        money += earned*mult;
        earned = 0;
        PlayerPrefs.SetInt("Money", money);
        GameEvents.current.MoneyChange();
        CheckMoney();
    }

    public void HandleDefeat() {
        ResetGame();
        GameEvents.current.HandleDefeat();
    }

    void SkipLevel() {
        level++;
        HandleTheme();
        PlayerPrefs.SetInt("Level", level);
        GameEvents.current.LevelChange(level);
        calculateDifficulty(level);
    }

    void HandleReward() {
        earned += Random.Range(1, income+5);
    }

    public void ResetGame() {
        StopCoroutine(StartedSpawning);
        StartedSpawning = null;
        playGame = false;
        playerAnimator.SetBool("Swing", false);
        Enemy.ResetStatics();
        GameEvents.current.DeleteAnimation();
        GameObject[] e = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject i in e) {
            Destroy(i);
        }
    }

    void HandleCosmetic(int type, int i, int cost) {
        if (cost <= money) {
            money -= cost;
            PlayerPrefs.SetInt("Money", money);
            GameEvents.current.MoneyChange();
            GameEvents.current.AnswerRequest(true, type, i);
            cosmetics[type,i,0] = 1;
        } else {
            GameEvents.current.AnswerRequest(false, type, i);
        }
    }

    void HandleCosmeticState(int type, int i, int state) {
        cosmetics[type,i,1] = state;
    }

    public void CheckMoney() {
        bool disable1 = false;
        bool disable2 = false;
        bool disable3 = false;
        if (money < fireRateCost) {
            disable1 = true;
        }
        if (money < ballBounceCost) {
            disable2 = true;
        }
        if (money < incomeCost) {
            disable3 = true;
        }
        GameEvents.current.CheckAfford(disable1, disable2, disable3);
    }

    void LoadTrail (Gradient trail) {
        currTrail = trail;
    }

    void SetPlayerMaterial(Material mat) {
        defaultMat = mat;
    }

    void SendCosmeticStates() {
        GameEvents.current.SendCosmeticStates(cosmetics);
    }

    void HandleTheme() {
        if (level % 2 == 0 && !themeSet) {
            themeSet = true;
            int n;
            do {
                n = Random.Range(0,4);
            } while (n == currTheme);
            currTheme = n;
            GameEvents.current.SetTheme(n);
        }
    }
}
}