using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Cinemachine;
namespace UnityEngine
{
    public class Manager : Singleton<Manager>
    {
        [SerializeField] GameObject player;
        [SerializeField] GameObject enemy;
        [SerializeField] GameObject bossEnemy;
        [SerializeField] GameObject cartEnemy;
        [SerializeField] GameObject shieldEnemy;
        [SerializeField] GameObject projectile;
        [SerializeField] GameObject powerUpProjectile;
        [SerializeField] GameObject spawner;
        [SerializeField] GameObject runWay;
        [SerializeField] GameObject[] powerBoxes;
        [SerializeField] Material defaultMat;
        [SerializeField] GameObject allyPrefab;
        [SerializeField] Transform[] allyPositions;
        [SerializeField] Transform bossPosition;
        [SerializeField] GameObject shield;
        [SerializeField] int shieldHP;
        [SerializeField] GameObject fireBall;
        EnemyClass EnemyClass;
        Animator playerAnimator;
        private bool playerDead;

        //Settings
        int soundEnabled = 1;
        int hapticsEnabled = 1;

        //Power Up
        GameObject[] enemies;
        GameObject[] powerUpProjectiles;
        private GameObject bossInstance;

        //Shooting
        float beganHolding = 0f;
        float pot = 0f;
        float fireRateCounter = 0f;
        int powerUpReq = 5;

        //Enemy Spawning
        IEnumerator StartedSpawning;
        int enemiesToSpawn = 0;
        float enemySpawnInterval = 1f;
        private int allyCount;
        Shield shieldData;
        private bool isBossFight;

        //Upgrades
        [SerializeField, Range(0, 1)] private float powerBoxSpawnChance;
        [SerializeField] private int powerBoxSpawnInterval;
        [SerializeField] Transform[] enemySpawnPositions;
        [SerializeField] BoxCollider enemySpawnBounds;
        private GameObject currentPowerBox;
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
        int[,,] cosmetics = new int[3, 7, 2];
        Gradient currTrail;


        //Game loop
        bool playGame = false;
        bool themeSet = false;
        int currTheme;
        int level = 1;

        //Getters/Setters
        public bool PlayerDead => playerDead;
        public bool PlayGame { set { playGame = value; } }
        public int SoundEnabled { get { return soundEnabled; } set { soundEnabled = value; } }
        public int HapticsEnabled { get { return hapticsEnabled; } set { hapticsEnabled = value; } }
        public int MaxBounces { get { return maxBounces; } }
        public int EnemiesToSpawn { get { return enemiesToSpawn; } }
        public int Money { get => money; }
        public int Level { get => level; }
        public int FireRateLevel { get => fireRateLevel; }
        public int FireRateCost { get => fireRateCost; }
        public int BallBounceLevel { get => ballBounceLevel; }
        public int BallBounceCost { get => ballBounceCost; }
        public int IncomeLevel { get => incomeLevel; }
        public int IncomeCost { get => incomeCost; }
        public int[,,] Cosmetics { get => cosmetics; }
        public int CurrTheme { get => currTheme; }
        public GameObject RunWay { get => runWay; set => runWay = value; }
        public GameObject Player => player;
        public GameObject BossInstance => bossInstance;

        void Start()
        {
            Handheld.Vibrate();
            shieldData = shield.GetComponent<Shield>();
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
            SpawnInitialEnemies();

            //Equip current cosmetics
            for (int i = 0; i < cosmetics.GetLength(0); i++)
            {
                for (int j = 0; j < cosmetics.GetLength(1); j++)
                {
                    if (cosmetics[i, j, 0] == 1 && cosmetics[i, j, 1] == 1)
                    {
                        GameEvents.current.EquipCurrent(i, j);
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
        Vector3 startPos, endPos;

        void Update()
        {
            //Gameplay
            if (Input.GetKeyDown(KeyCode.C))
                UpdateShield();
            if (Input.GetKeyDown(KeyCode.V))
                EnemyClass.KilledCount = 100;
            if (!playGame) return;

            if (StartedSpawning == null && !Enemy.isPassive)
            {
                StartedSpawning = SpawnEnemy();
                StartCoroutine(StartedSpawning);
                StartCoroutine(SpawnPowerBox());
            }
            else
            {
                //HANDLE VICTORY
                if (Input.GetKeyDown(KeyCode.X) || (EnemyClass.TotalKilledCount == EnemyClass.AliveCount && !isBossFight))
                {
                    // HandleVictory();
                    //put boss here and then handle victory.
                    isBossFight = true;
                    RemoveAllCharacters();
                    StopCoroutine(StartedSpawning);
                    GameObject boss = Instantiate(bossEnemy, bossPosition);
                    bossInstance = boss;
                    boss.GetComponent<BossEnemy>().OnKnockedOut += BossKnockedOut;
                }
            }

            // Shooting / Power up
            if (Input.GetMouseButtonDown(0))
            {
                beganHolding = Time.unscaledTime;
                Vector3 mousepos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                startPos = Camera.main.ScreenToWorldPoint(mousepos);
            }

            if (Input.GetMouseButton(0))
            {
                float holdTime = Time.unscaledTime - beganHolding;
                if (EnemyClass.KilledCount >= powerUpReq && holdTime > 0.1f)
                {
                    enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    pot += Time.unscaledDeltaTime;
                    GameEvents.current.AnimatePowerUp(enemies.Length, pot);
                    GameEvents.current.KillsChange(Mathf.Lerp(powerUpReq, 0, pot / 1f));
                }
                Vector3 mousepos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                endPos = Camera.main.ScreenToWorldPoint(mousepos);
                //if (endPos != startPos)
            }


            if (Input.GetMouseButtonUp(0))
            {

                float holdTime = Time.unscaledTime - beganHolding;
                if (holdTime >= 1f && EnemyClass.KilledCount >= powerUpReq)
                {
                    powerUpProjectiles = new GameObject[enemies.Length];
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        int r = 3;
                        float x = r * Mathf.Cos(i * (Mathf.PI / (enemies.Length)));
                        float y = r * Mathf.Sin(i * (Mathf.PI / (enemies.Length)));
                        powerUpProjectiles[i] = Instantiate(powerUpProjectile, spawner.transform.position + new Vector3(0, y, x), Quaternion.identity);
                        if (currTrail != null) powerUpProjectiles[i].GetComponent<TrailRenderer>().colorGradient = currTrail;
                        powerUpProjectiles[i].GetComponent<Rigidbody>().useGravity = true;
                        powerUpProjectiles[i].transform.LookAt(enemies[i].transform.position);
                        powerUpProjectiles[i].GetComponent<Rigidbody>().velocity = (powerUpProjectiles[i].transform.forward * 100f) / Time.timeScale;
                    }
                    EnemyClass.KilledCount = 0;
                    pot = 0;
                    GameEvents.current.DeleteAnimation();
                    GameEvents.current.PowerUp();
                }
                else
                {
                    if (calculateTarget() == Vector3.zero) return;
                    playerAnimator.SetBool("Swing", true);
                    pot = 0;
                    GameEvents.current.DeleteAnimation();
                    GameEvents.current.KillsChange(EnemyClass.KilledCount);
                }
            }

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                playerAnimator.SetBool("Swing", false);
            }

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
            {
                fireProjectile(ref fireRateCounter, Mathf.Lerp(0.8f, 0.4f, (float)fireRateLevel / upgradeMaxLevel));
            }
        }

        void BossKnockedOut(BossEnemy boss)
        {
            playerAnimator.SetBool("Swing", false);
            CutSceneHelper.I.DoPlayerCutScene(() => boss.transform.position = bossPosition.position);
            playGame = false;
            boss.OnKnockedOut -= BossKnockedOut;
        }

        // TESTING
        void OnApplicationFocus(bool hasFocus)
        {
            //SaveData();
        }

        // Functions

        void DeleteData()
        {
            SaveSystem.DeleteSaves();
        }

        void SaveData()
        {
            SaveSystem.SaveData(this);
        }

        void LoadData()
        {
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

        IEnumerator SpawnEnemy()
        {
            while (EnemyClass.AliveCount < enemiesToSpawn)
            {
                Bounds bounds = enemySpawnBounds.bounds;
                float chance = Random.Range(0f, 1f);
                float xPos = Random.Range(-bounds.extents.x, bounds.extents.x);
                float zPos = Random.Range(-bounds.extents.z, bounds.extents.z);

                GameObject selected = null;
                if (chance < 0.1f && level > 10)
                {
                    selected = cartEnemy;
                }
                else if (chance > 0.1f && chance < 0.3f && level > 5)
                {
                    selected = shieldEnemy;
                }
                else
                {
                    selected = enemy;
                }
                GameObject enemyGO = Instantiate(selected, bounds.center + new Vector3(xPos, 30, zPos), Quaternion.identity);
                yield return new WaitForSecondsRealtime(1);
            }
        }

        IEnumerator SpawnPowerBox()
        {
            int zPos = Random.Range(-76, -25);
            float xPos = Random.Range(0.05f, 0.95f);
            Vector3 worldPos = new Vector3(xPos, 0, Mathf.Abs(Camera.main.transform.position.z - zPos));
            worldPos = Camera.main.ViewportToWorldPoint(worldPos);
            while (currentPowerBox == null && playGame)
            {
                yield return new WaitForSeconds(powerBoxSpawnInterval);
                if (powerBoxSpawnChance > Random.value)
                {
                    var col = runWay.GetComponent<Collider>();
                    var arrayToUse = powerBoxes;
                    GameObject powerBoxToSpawn;
                    if (allyCount > 2)
                    {
                        var arr = powerBoxes.Where(p => !p.GetComponent<AllyBox>()).ToArray();
                        arrayToUse = arr;
                    }
                    powerBoxToSpawn = arrayToUse[Random.Range(0, arrayToUse.Length)];
                    Vector3 randPos = new Vector3(worldPos.x, 1.8f, zPos);
                    currentPowerBox = Instantiate(powerBoxToSpawn, randPos, Quaternion.Euler(-90, 0, 0));
                    currentPowerBox.GetComponent<PowerBox>().OnDestroyed.AddListener(() => StartCoroutine(SpawnPowerBox()));
                }
            }
        }

        //FIRE PROJECTILES
        void fireProjectile(ref float fireRateCounter, float nextFire)
        {
            if (Time.unscaledTime > fireRateCounter)
            {
                fireRateCounter = Time.unscaledTime + nextFire;
                Vector3 flatAimTarget = calculateTarget();
                GameObject p = Instantiate(projectile, spawner.transform.position, Quaternion.identity);
                if (currTrail != null) p.GetComponent<TrailRenderer>().colorGradient = currTrail;
                p.transform.LookAt(flatAimTarget);
                p.GetComponent<Rigidbody>().useGravity = true;
                p.GetComponent<Projectile>().SetForce();
            }
        }

        Vector3 calculateTarget()
        {
            Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            LayerMask mask = 1 << 11; //11 is player's layer
            mask = ~mask; // hit everything but the player's layer
            Physics.Raycast(Camera.main.transform.position, cursorRay.direction, out RaycastHit hit, Mathf.Infinity, mask);
            Vector3 dir = new Vector3(hit.point.x, 0.63f, hit.point.z);
            Debug.Log(dir);
            return dir.z >= player.transform.position.z ? dir : Vector3.zero;
        }

        void calculateDifficulty(int level)
        {
            enemiesToSpawn = Mathf.Clamp((int)Mathf.Ceil((level + 10) * 1.2f), 0, 300);
            enemySpawnInterval = Mathf.Clamp(1 - level / 100f, 0.2f, 1f);
        }

        void UpdateUpgradeValues()
        {
            fireRate = Mathf.Lerp(1.0f, 5.0f, (float)fireRateLevel / upgradeMaxLevel);
            maxBounces = ballBounceLevel;
            playerAnimator.SetFloat("Speed", fireRate);
            income = incomeLevel;
        }

        public void SpawnAlly()
        {
            allyCount++;
            Vector3 allyStartPos = allyCount == 1 ? allyPositions[0].position : allyPositions[1].position;
            allyStartPos += new Vector3(0, 20, 0);//putting offset to spawn midair
            Instantiate(allyPrefab, allyStartPos, Quaternion.Euler(0, 180, 0));
        }

        public void UpdateShield()
        {
            shieldData.Health = shieldHP;
            shield.SetActive(true);
            shieldData.InitShield();
        }

        public void DoFinalSwing(float sliderForce)
        {
            playerAnimator.SetBool("Swing", true);
            StartCoroutine(WaitForFinalSwing(sliderForce));
        }

        IEnumerator WaitForFinalSwing(float forceAmount)
        {
            yield return new WaitUntil(() => playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);
            GameObject ballFire = Instantiate(fireBall, spawner.transform.position, Quaternion.identity);
            ballFire.transform.LookAt(bossInstance.transform);
            var fireRb = ballFire.GetComponent<Rigidbody>();
            fireRb.AddForce((ballFire.transform.forward * 25f + ballFire.transform.up * 2), ForceMode.Impulse);
            ballFire.GetComponent<FireBall>().SliderForce = forceAmount;
            CutSceneHelper.I.SetBallCamFocus(ballFire.transform);
        }

        // EVENT FUNCTIONS
        public void HandleUpgrade1()
        {
            if (money > fireRateCost && fireRateLevel < upgradeMaxLevel)
            {
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

        public void HandleUpgrade2()
        {
            if (money > ballBounceCost && ballBounceLevel < 5)
            {
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

        public void HandleUpgrade3()
        {
            if (money > incomeCost && incomeLevel < upgradeMaxLevel)
            {
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

        public void HandleVictory(float multiplier)
        {
            themeSet = false;
            ResetGame();
            level++;
            PlayerPrefs.SetInt("Level", level);
            calculateDifficulty(level);
            GameEvents.current.HandleVictory(level, (int)(earned * multiplier));
        }

        void RewardVictory(int mult)
        {
            HandleTheme();
            money += earned * mult;
            earned = 0;
            PlayerPrefs.SetInt("Money", money);
            GameEvents.current.MoneyChange();
            CheckMoney();
        }

        public void HandleDefeat()
        {
            //ResetGame();
            playerDead = true;
            playerAnimator.SetTrigger("Hit");
            player.transform.localEulerAngles = new Vector3(-90, 0, 0);
            var playerLaunchDestination = new Vector3(1, 0, Random.Range(-1f, 0f)) * 10;
            player.transform.DOMove(new Vector3(0, .8f) + player.transform.localPosition + playerLaunchDestination, 3).OnComplete(()
            => GameEvents.current.HandleDefeat());
        }

        void SkipLevel()
        {
            level++;
            HandleTheme();
            PlayerPrefs.SetInt("Level", level);
            GameEvents.current.LevelChange(level);
            calculateDifficulty(level);
        }

        void HandleReward()
        {
            earned += Random.Range(1, income + 5);
        }

        void SpawnInitialEnemies()
        {
            for (int i = 0; i < enemySpawnPositions.Length; i++)
            {
                Instantiate(enemy, enemySpawnPositions[i].position, Quaternion.identity);//fix later
            }
        }

        public void ResetGame()
        {
            playerDead = false;
            StopCoroutine(StartedSpawning);
            StartedSpawning = null;
            playGame = false;
            isBossFight = false;
            allyCount = 0;
            playerAnimator.SetBool("Swing", false);
            Enemy.ResetStatics();
            GameEvents.current.DeleteAnimation();
            player.transform.localPosition = new Vector3(125.4778f, 1.418f, -88.17085f);
            SpawnInitialEnemies();
            RemoveAllCharacters();
        }

        void RemoveAllCharacters()
        {
            if (shield.activeSelf)
                shield.GetComponent<Shield>().DestroyShield();
            if (bossInstance)
                Destroy(bossInstance);
            GameObject[] e = GameObject.FindGameObjectsWithTag("Enemy");
            e = e.Concat(GameObject.FindGameObjectsWithTag("Ally")).ToArray();
            foreach (GameObject i in e)
            {
                Destroy(i);
            }
        }

        void HandleCosmetic(int type, int i, int cost)
        {
            if (cost <= money)
            {
                money -= cost;
                PlayerPrefs.SetInt("Money", money);
                GameEvents.current.MoneyChange();
                GameEvents.current.AnswerRequest(true, type, i);
                cosmetics[type, i, 0] = 1;
            }
            else
            {
                GameEvents.current.AnswerRequest(false, type, i);
            }
        }

        void HandleCosmeticState(int type, int i, int state)
        {
            cosmetics[type, i, 1] = state;
        }

        public void CheckMoney()
        {
            bool disable1 = false;
            bool disable2 = false;
            bool disable3 = false;
            if (money < fireRateCost)
            {
                disable1 = true;
            }
            if (money < ballBounceCost)
            {
                disable2 = true;
            }
            if (money < incomeCost)
            {
                disable3 = true;
            }
            GameEvents.current.CheckAfford(disable1, disable2, disable3);
        }

        void LoadTrail(Gradient trail)
        {
            currTrail = trail;
        }

        void SetPlayerMaterial(Material mat)
        {
            defaultMat = mat;
        }

        void SendCosmeticStates()
        {
            GameEvents.current.SendCosmeticStates(cosmetics);
        }

        void HandleTheme()
        {
            if (level % 2 == 0 && !themeSet)
            {
                themeSet = true;
                int n;
                do
                {
                    n = Random.Range(0, 4);
                } while (n == currTheme);
                currTheme = n;
                GameEvents.current.SetTheme(n);
            }
        }
    }
}