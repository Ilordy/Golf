using UnityEngine;
using System.Collections;


public class Manager : MonoBehaviour {

    public PowerUpAnimation PowerUpAnimation;
    private EnemyClass EnemyClass;

    public GameObject player;
    public GameObject enemy;
    public GameObject cartEnemy;
    public GameObject shieldEnemy;
    public GameObject projectile;
    public GameObject powerUpProjectile;
    public GameObject spawner;

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
    float   currentTime = 0f;

    //Upgrades
    float fireRate = 0.5f;
    int maxBounces = 4;
    int income = 1;
    int fireRateLevel = 1;
    int fireRateCost = 100;
    int ballBounceLevel = 4;
    int ballBounceCost = 100;
    int incomeLevel = 1;
    int incomeCost = 100;
    int upgradeMaxLevel = 20;
    int money = 0;
    public int Money {get;}
    int earned = 0;

    //Game loop
    public bool playGame = false;
    public bool willDeleteSaves = false;
    [SerializeField] int level = 1;

    //Properties
    public int SoundEnabled {get;set;}
    public int HapticsEnabled {get;set;}
    public int MaxBounces {get{return maxBounces;}}
    public int Level {get;}


    void Start() {
        //Debug delete
        if(willDeleteSaves) {
            PlayerPrefs.DeleteAll();
        }

        //Get EnemyClass
        EnemyClass = GetComponent<EnemyClass>();
        //Set Player Animator
        playerAnimator = player.GetComponent<Animator>();

        //Get all persistent values
        level = PlayerPrefs.GetInt("Level", 1);
        money = PlayerPrefs.GetInt("Money", 0);
        fireRateLevel = PlayerPrefs.GetInt("FireRateLevel", 1);
        fireRateCost = PlayerPrefs.GetInt("FireRateCost", 100);
        ballBounceLevel = PlayerPrefs.GetInt("BallBounceLevel", 1);
        ballBounceCost = PlayerPrefs.GetInt("BallBounceCost", 1000);
        incomeLevel = PlayerPrefs.GetInt("IncomeLevel", 1);
        incomeCost = PlayerPrefs.GetInt("IncomeCost", 100);
        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1);
        hapticsEnabled = PlayerPrefs.GetInt("HapticsEnabled", 1);

        //Calculate difficulty for current level
        calculateDifficulty(level);
        
        //Update Upgrade Values
        UpdateUpgradeValues();

        //Update UI
        money = 5000000;
        GameEvents.current.SettingsChange();
        GameEvents.current.LevelChange(level);
        GameEvents.current.MoneyChange(money);
        GameEvents.current.UpgradesChange(1, fireRateLevel, fireRateCost);
        GameEvents.current.UpgradesChange(2, ballBounceLevel, ballBounceCost);
        GameEvents.current.UpgradesChange(3, incomeLevel, incomeCost);
    }

    void Update() {
        //Gameplay
        if (!playGame) return;

        if (StartedSpawning == null) {
            StartedSpawning = SpawnEnemy();
            StartCoroutine(SpawnEnemy());
        } else {
            //HANDLE VICTORY
            if (EnemyClass.GetData("TotalKilledCount") == EnemyClass.GetData("AliveCount")) {
                HandleVictory();
            }
        }

        // Adjust Power Up Slider
        if (EnemyClass.GetData("KilledCount") <= powerUpReq) {
            GameEvents.current.KillsChange(EnemyClass.GetData("KilledCount"));
        }

        // Shooting / Power up
        if (Input.GetMouseButtonDown(0)) {
            beganHolding = Time.time;
        }

        if (Input.GetMouseButton(0)) {
            float holdTime = Time.time - beganHolding;
            if (EnemyClass.GetData("KilledCount") >= powerUpReq && holdTime > 0.1f) {
                enemies = GameObject.FindGameObjectsWithTag("Enemy");
                pot += Time.deltaTime;
                PowerUpAnimation.AnimatePowerUp(enemies.Length, pot);
                GameEvents.current.KillsChange(Mathf.Lerp(powerUpReq, 0, pot / 1f));
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            float holdTime = Time.time - beganHolding;
            if (holdTime >= 1f && EnemyClass.GetData("KilledCount") >= powerUpReq) {
                //enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
                EnemyClass.SetKilledCount(0);
                pot = 0;
                PowerUpAnimation.DeleteAnimation();
            } else {
                playerAnimator.SetBool("Swing",false);
                playerAnimator.SetBool("Swing",true);
                pot = 0;
                PowerUpAnimation.DeleteAnimation();
                GameEvents.current.KillsChange(EnemyClass.GetData("KilledCount"));
            }
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            playerAnimator.SetBool("Swing",false);
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f) {
            fireProjectile(ref fireRateCounter, Mathf.Lerp(0.8f,0.4f, (float)fireRateLevel/upgradeMaxLevel));
        }
    }

    // Functions

    IEnumerator SpawnEnemy() {
        while (EnemyClass.GetData("AliveCount") < enemiesToSpawn) {
            float chance = Random.Range(0f,1f);
            float xPos = Random.Range(-47f,-32f);
            float zPos = Random.Range(-3.65f,3.65f);
            GameObject selected = null;
            if (chance < 0.1f) {
                selected = cartEnemy;
            } else if (chance > 0.1f && chance < 0.9f) {
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
        enemiesToSpawn = (int)Mathf.Ceil((level + 10) * 1.2f);
        enemySpawnInterval = Mathf.Clamp(1-level/100f, 0.2f,1f);
    }

    void UpdateUpgradeValues() {
        fireRate = Mathf.Lerp(1.0f,5.0f,(float)fireRateLevel/upgradeMaxLevel);
        maxBounces = ballBounceLevel;
        playerAnimator.SetFloat("Speed", fireRate);
        income = incomeLevel;
    }

    void HandleVictory() {
        StartedSpawning = null;
        playGame = false;
        playerAnimator.SetBool("Swing", false);
        level++;
        PlayerPrefs.SetInt("Level", level);
        calculateDifficulty(level);
        GameEvents.current.HandleVictory(level, earned);
        Enemy.ResetStatics();
        PowerUpAnimation.DeleteAnimation();
    }

    public void HandleDefeat() {
        PowerUpAnimation.DeleteAnimation();
        playGame = false;
        playerAnimator.SetBool("Swing", false);
        GameEvents.current.HandleDefeat();
        Enemy.ResetStatics();
        GameObject[] e = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject i in e) {
            Destroy(i);
        }
    }

    public void SkipLevel() {
        level++;
        PlayerPrefs.SetInt("Level", level);
        GameEvents.current.LevelChange(level);
        calculateDifficulty(level);
    }

    public void HandleUpgrade1() {
        if (money > fireRateCost && fireRateLevel < upgradeMaxLevel) {
            money -= fireRateCost;
            fireRateLevel++;
            fireRateCost += 100;
        }
        UpdateUpgradeValues();
        GameEvents.current.UpgradesChange(1, fireRateLevel, fireRateCost);
        GameEvents.current.MoneyChange(money);
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
        UpdateUpgradeValues();
        GameEvents.current.UpgradesChange(2, ballBounceLevel, ballBounceCost);
        GameEvents.current.MoneyChange(money);
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
        UpdateUpgradeValues();
        GameEvents.current.UpgradesChange(3, incomeLevel, incomeCost);
        GameEvents.current.MoneyChange(money);
        PlayerPrefs.SetInt("IncomeLevel", incomeLevel);
        PlayerPrefs.SetInt("IncomeCost", incomeCost);
        PlayerPrefs.SetInt("Money", money);
    }

    public void HandleReward() {
        earned += Random.Range(1, income);
    }

    public void AwardRegular() {
        money += earned;
        earned = 0;
        PlayerPrefs.SetInt("Money", money);
        GameEvents.current.MoneyChange(money);
    }

    public void AwardDouble() {
        money += earned*2;
        earned = 0;
        PlayerPrefs.SetInt("Money", money);
        GameEvents.current.MoneyChange(money);
    }
}