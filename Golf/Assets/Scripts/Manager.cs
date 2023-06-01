using System.Collections;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Manager : Singleton<Manager>
{
    [FormerlySerializedAs("player")] [SerializeField]
    GameObject m_player;

    [FormerlySerializedAs("enemy")] [SerializeField]
    GameObject m_enemy;

    [FormerlySerializedAs("bossEnemy")] [SerializeField]
    GameObject m_bossEnemy;

    [FormerlySerializedAs("cartEnemy")] [SerializeField]
    GameObject m_cartEnemy;

    [FormerlySerializedAs("shieldEnemy")] [SerializeField]
    GameObject m_shieldEnemy;

    [FormerlySerializedAs("projectile")] [SerializeField]
    GameObject m_projectile;

    [FormerlySerializedAs("powerUpProjectile")] [SerializeField]
    GameObject m_powerUpProjectile;

    [FormerlySerializedAs("spawner")] [SerializeField]
    GameObject m_spawner;

    [FormerlySerializedAs("runWay")] [SerializeField]
    GameObject m_runWay;

    [FormerlySerializedAs("powerBoxes")] [SerializeField]
    GameObject[] m_powerBoxes;

    [FormerlySerializedAs("defaultMat")] [SerializeField]
    Material m_defaultMat;

    [FormerlySerializedAs("allyPrefab")] [SerializeField]
    GameObject m_allyPrefab;

    [FormerlySerializedAs("allyPositions")] [SerializeField]
    Transform[] m_allyPositions;

    [FormerlySerializedAs("bossPosition")] [SerializeField]
    Transform m_bossPosition;

    [FormerlySerializedAs("shield")] [SerializeField]
    GameObject m_shield;

    [FormerlySerializedAs("shieldHP")] [SerializeField]
    int m_shieldHp;

    [FormerlySerializedAs("fireBall")] [SerializeField]
    GameObject m_fireBall;

    Animator m_playerAnimator;
    bool m_playerDead;

    //Settings
    int m_soundEnabled = 1;
    int m_hapticsEnabled = 1;

    //Power Up
    GameObject[] m_enemies;
    GameObject[] m_powerUpProjectiles;
    GameObject m_bossInstance;
    EnemyPooler m_enemyPooler;
    GolfBallPooler projectilePooler;

    //Shooting
    float m_beganHolding = 0f;
    float m_pot = 0f;
    float m_fireRateCounter = 0f;
    int m_powerUpReq = 5;

    //Enemy Spawning
    IEnumerator m_startedSpawning;
    public int m_enemiesToSpawn = 0;
    float m_enemySpawnInterval = 1f;
    int m_allyCount;
    Shield m_shieldData;
    bool m_isBossFight;

    //Upgrades
    [FormerlySerializedAs("powerBoxSpawnChance")] [SerializeField] [Range(0, 1)]
    float m_powerBoxSpawnChance;

    [FormerlySerializedAs("powerBoxSpawnInterval")] [SerializeField]
    int m_powerBoxSpawnInterval;

    [FormerlySerializedAs("enemySpawnPositions")] [SerializeField]
    Transform[] m_enemySpawnPositions;

    [FormerlySerializedAs("enemySpawnBounds")] [SerializeField]
    BoxCollider m_enemySpawnBounds;

    GameObject m_currentPowerBox;
    float m_fireRate = 0.5f;
    int m_maxBounces = 4;
    int m_income = 1;
    int m_fireRateLevel = 1;
    int m_fireRateCost = 100;
    int m_ballBounceLevel = 1;
    int m_ballBounceCost = 1000;
    int m_incomeLevel = 1;
    int m_incomeCost = 100;
    int m_upgradeMaxLevel = 20;
    int m_money = 0;
    int m_earned = 0;
    Camera mainCam;

    //Cosmetics
    int[,,] m_cosmetics = new int[3, 7, 2];
    Gradient m_currTrail;

    //Game loop
    bool m_playGame = false;
    bool m_themeSet = false;
    int m_currTheme;
    int m_level = 1;

    //Getters/Setters
    public bool PlayerDead => m_playerDead;

    public bool PlayGame
    {
        set => m_playGame = value;
    }

    public int SoundEnabled
    {
        get => m_soundEnabled;
        set => m_soundEnabled = value;
    }

    public int HapticsEnabled
    {
        get => m_hapticsEnabled;
        set => m_hapticsEnabled = value;
    }

    public int MaxBounces => m_maxBounces;

    public int EnemiesToSpawn => m_enemiesToSpawn;

    public int Money => m_money;

    public int Level => m_level;

    public int FireRateLevel => m_fireRateLevel;

    public int FireRateCost => m_fireRateCost;

    public int BallBounceLevel => m_ballBounceLevel;

    public int BallBounceCost => m_ballBounceCost;

    public int IncomeLevel => m_incomeLevel;

    public int IncomeCost => m_incomeCost;

    public int[,,] Cosmetics => m_cosmetics;

    public int CurrTheme => m_currTheme;

    public GameObject RunWay
    {
        get => m_runWay;
        set => m_runWay = value;
    }

    public GameObject Player => m_player;
    public GameObject BossInstance => m_bossInstance;

    public GolfBallPooler ProjectilePooler => projectilePooler;

    void Start()
    {
        mainCam = Camera.main;
        m_enemyPooler = GetComponent<EnemyPooler>();
        projectilePooler = GetComponent<GolfBallPooler>();
        Handheld.Vibrate();
        m_shieldData = m_shield.GetComponent<Shield>();
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
        //Set Player Animator
        m_playerAnimator = m_player.GetComponent<Animator>();

        //LOAD GAME DATA
        //DeleteData();
        //LoadData();
        m_money = 20000;

        //Set player default material
        m_player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial = m_defaultMat;

        //Calculate difficulty for current level
        CalculateDifficulty(m_level);

        //Update Upgrade Values
        UpdateUpgradeValues();
        CheckMoney();
        SpawnInitialEnemies();

        //Equip current cosmetics
        for (var i = 0; i < m_cosmetics.GetLength(0); i++)
        for (var j = 0; j < m_cosmetics.GetLength(1); j++)
            if (m_cosmetics[i, j, 0] == 1 && m_cosmetics[i, j, 1] == 1)
            {
                GameEvents.current.EquipCurrent(i, j);
                break;
            }

        //Update UI
        GameEvents.current.SettingsChange();
        GameEvents.current.LevelChange(m_level);
        GameEvents.current.MoneyChange();
        GameEvents.current.UpgradesChange(1, m_fireRateLevel, m_fireRateCost);
        GameEvents.current.UpgradesChange(2, m_ballBounceLevel, m_ballBounceCost);
        GameEvents.current.UpgradesChange(3, m_incomeLevel, m_incomeCost);
    }

    Vector3 m_startPos, m_endPos;
    public int killCount;
    public int aliveCount;
    void Update()
    {
        killCount = EnemyClass.TotalKilledCount;
        aliveCount = EnemyClass.AliveCount;
        //Debug.Log(Mathf.InverseLerp(0, 100, m_level));
        //Gameplay
        if (Input.GetKeyDown(KeyCode.C)) UpdateShield();
        if (Input.GetKeyDown(KeyCode.V)) EnemyClass.KilledCount = 100;
        if (!m_playGame) return;
        if (m_startedSpawning == null && !Enemy.isPassive)
        {
            m_startedSpawning = SpawnEnemy();
            StartCoroutine(m_startedSpawning);
            StartCoroutine(SpawnPowerBox());
        }
        else
        {
            //HANDLE VICTORY
            if (Input.GetKeyDown(KeyCode.X) || (EnemyClass.TotalKilledCount == EnemyClass.AliveCount && !m_isBossFight))
            {
                // HandleVictory();
                //put boss here and then handle victory.
                m_isBossFight = true;
                RemoveAllCharacters();
                StopCoroutine(m_startedSpawning);
                var boss = Instantiate(m_bossEnemy, m_bossPosition);
                m_bossInstance = boss;
                 boss.GetComponent<BossEnemy>().OnKnockedOut += BossKnockedOut;
            }
        }

        // Shooting / Power up
        if (Input.GetMouseButtonDown(0))
        {
            m_beganHolding = Time.unscaledTime;
            var mousepos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            m_startPos = mainCam.ScreenToWorldPoint(mousepos);
        }

        if (Input.GetMouseButton(0))
        {
            var holdTime = Time.unscaledTime - m_beganHolding;
            if (EnemyClass.KilledCount >= m_powerUpReq && holdTime > 0.1f)
            {
                m_enemies = GameObject.FindGameObjectsWithTag("Enemy");
                m_pot += Time.unscaledDeltaTime;
                GameEvents.current.AnimatePowerUp(m_enemies.Length, m_pot);
                GameEvents.current.KillsChange(Mathf.Lerp(m_powerUpReq, 0, m_pot / 1f));
            }

            var mousepos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            m_endPos = mainCam.ScreenToWorldPoint(mousepos);
            //if (endPos != startPos)
        }

        if (Input.GetMouseButtonUp(0))
        {
            var holdTime = Time.unscaledTime - m_beganHolding;
            if (holdTime >= 1f && EnemyClass.KilledCount >= m_powerUpReq)
            {
                m_powerUpProjectiles = new GameObject[m_enemies.Length];
                for (var i = 0; i < m_enemies.Length; i++)
                {
                    var r = 3;
                    var x = r * Mathf.Cos(i * (Mathf.PI / m_enemies.Length));
                    var y = r * Mathf.Sin(i * (Mathf.PI / m_enemies.Length));
                    m_powerUpProjectiles[i] = Instantiate(m_powerUpProjectile,
                        m_spawner.transform.position + new Vector3(0, y, x), Quaternion.identity);
                    if (m_currTrail != null)
                        m_powerUpProjectiles[i].GetComponent<TrailRenderer>().colorGradient = m_currTrail;
                    m_powerUpProjectiles[i].GetComponent<Rigidbody>().useGravity = true;
                    m_powerUpProjectiles[i].transform.LookAt(m_enemies[i].transform.position);
                    m_powerUpProjectiles[i].GetComponent<Rigidbody>().velocity =
                        m_powerUpProjectiles[i].transform.forward * 100f / Time.timeScale;
                }

                EnemyClass.KilledCount = 0;
                m_pot = 0;
                GameEvents.current.DeleteAnimation();
                GameEvents.current.PowerUp();
            }
            else
            {
                if (CalculateTarget() == Vector3.zero) return;
                m_playerAnimator.SetBool("Swing", true);
                m_pot = 0;
                GameEvents.current.DeleteAnimation();
                GameEvents.current.KillsChange(EnemyClass.KilledCount);
            }
        }

        if (m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") &&
            m_playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            m_playerAnimator.SetBool("Swing", false);

        if (m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") &&
            m_playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
            FireProjectile(ref m_fireRateCounter, Mathf.Lerp(0.8f, 0.4f, (float)m_fireRateLevel / m_upgradeMaxLevel));
    }

    void BossKnockedOut(BossEnemy boss)
    {
        m_playerAnimator.SetBool("Swing", false);
        CutSceneHelper.I.DoPlayerCutScene(() => boss.transform.position = m_bossPosition.position);
        m_playGame = false;
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
        var data = SaveSystem.LoadData();
        if (data == null) return;
        m_level = data.level;
        m_money = data.money;
        m_fireRateLevel = data.fireRateLevel;
        m_fireRateCost = data.fireRateCost;
        m_ballBounceLevel = data.ballBounceLevel;
        m_ballBounceCost = data.ballBounceCost;
        m_incomeLevel = data.incomeLevel;
        m_incomeCost = data.incomeCost;
        m_soundEnabled = data.soundEnabled;
        m_hapticsEnabled = data.hapticsEnabled;
        m_cosmetics = data.cosmetics;
        m_currTheme = data.currTheme;
    }

    [SerializeField] float m_baseSpawnInterval;

    IEnumerator SpawnEnemy()
    {
        while (EnemyClass.AliveCount < m_enemiesToSpawn + 50)
        {
            var spawnInterval = Mathf.Log(EnemyClass.AliveCount - m_enemySpawnPositions.Length + 1, 2) *
                                m_baseSpawnInterval;
            m_enemyPooler.SpawnRandomEnemy();
            //TODO Refactor this code to be more readable and maintainable
            //TODO make algorithm for spawning enemies with probability! 
            //spawn enemies with probability based on level
            yield return new WaitForSecondsRealtime(spawnInterval);
        }
    }

    IEnumerator SpawnPowerBox()
    {
        var zPos = Random.Range(-76, -25);
        var xPos = Random.Range(0.05f, 0.95f);
        var worldPos = new Vector3(xPos, 0, Mathf.Abs(mainCam.transform.position.z - zPos));
        worldPos = mainCam.ViewportToWorldPoint(worldPos);
        while (m_currentPowerBox == null && m_playGame)
        {
            yield return new WaitForSeconds(m_powerBoxSpawnInterval);
            if (m_powerBoxSpawnChance > Random.value)
            {
                var col = m_runWay.GetComponent<Collider>();
                var arrayToUse = m_powerBoxes;
                GameObject powerBoxToSpawn;
                if (m_allyCount > 2)
                {
                    var arr = m_powerBoxes.Where(p => !p.GetComponent<AllyBox>()).ToArray();
                    arrayToUse = arr;
                }

                powerBoxToSpawn = arrayToUse[Random.Range(0, arrayToUse.Length)];
                var randPos = new Vector3(worldPos.x, 1.8f, zPos);
                m_currentPowerBox = Instantiate(powerBoxToSpawn, randPos, Quaternion.Euler(-90, 0, 0));
                m_currentPowerBox.GetComponent<PowerBox>().OnDestroyed
                    .AddListener(() => StartCoroutine(SpawnPowerBox()));
            }
        }
    }

    //FIRE PROJECTILES
    void FireProjectile(ref float fireRateCounter, float nextFire)
    {
        if (Time.unscaledTime > fireRateCounter)
        {
            fireRateCounter = Time.unscaledTime + nextFire;
            var flatAimTarget = CalculateTarget();
            var p = ProjectilePooler.Get();
            p.transform.position = m_spawner.transform.position;
            p.transform.LookAt(flatAimTarget);
            if (m_currTrail != null)
                p.Trail.colorGradient = m_currTrail;
        }
    }
    
    Vector3 CalculateTarget()
    {
        var cursorRay = mainCam.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = 1 << 11; //11 is player's layer
        mask = ~mask; // hit everything but the player's layer
        Physics.Raycast(mainCam.transform.position, cursorRay.direction, out var hit, Mathf.Infinity, mask);
        var dir = new Vector3(hit.point.x, 0.63f, hit.point.z);
        return dir.z >= m_player.transform.position.z ? dir : Vector3.zero;
    }

    void CalculateDifficulty(int level)
    {
        m_enemiesToSpawn = Mathf.Clamp((int)Mathf.Ceil((level + 10) * 1.2f), 1, 300);
        m_enemySpawnInterval = Mathf.Clamp(1 - level / 100f, 0.2f, 1f);
    }

    void UpdateUpgradeValues()
    {
        m_fireRate = Mathf.Lerp(1.0f, 5.0f, (float)m_fireRateLevel / m_upgradeMaxLevel);
        m_maxBounces = m_ballBounceLevel;
        m_playerAnimator.SetFloat("Speed", m_fireRate);
        m_income = m_incomeLevel;
    }

    public Vector3 GetEnemySpawnPoint() //For enemies
    {
        var bounds = m_enemySpawnBounds.bounds;
        var chance = Random.Range(0f, 1f);
        var xPos = Random.Range(-bounds.extents.x, bounds.extents.x);
        var zPos = Random.Range(-bounds.extents.z, bounds.extents.z);
        return bounds.center + new Vector3(xPos, 30, zPos);
    }

    public void SpawnAlly()
    {
        m_allyCount++;
        var allyStartPos = m_allyCount == 1 ? m_allyPositions[0].position : m_allyPositions[1].position;
        allyStartPos += new Vector3(0, 20, 0); //putting offset to spawn midair
        Instantiate(m_allyPrefab, allyStartPos, Quaternion.Euler(0, 180, 0));
    }

    public void UpdateShield()
    {
        m_shieldData.Health = m_shieldHp;
        m_shield.SetActive(true);
        m_shieldData.InitShield();
    }

    public void DoFinalSwing(float sliderForce)
    {
        m_playerAnimator.SetBool("Swing", true);
        StartCoroutine(WaitForFinalSwing(sliderForce));
    }

    IEnumerator WaitForFinalSwing(float forceAmount)
    {
        yield return new WaitUntil(() =>
            m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("drive") &&
            m_playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);
        var ballFire = Instantiate(m_fireBall, m_spawner.transform.position, Quaternion.identity);
        ballFire.transform.LookAt(m_bossInstance.transform);
        var fireRb = ballFire.GetComponent<Rigidbody>();
        fireRb.AddForce(ballFire.transform.forward * 25f + ballFire.transform.up * 2, ForceMode.Impulse);
        ballFire.GetComponent<FireBall>().SliderForce = forceAmount;
        CutSceneHelper.I.SetBallCamFocus(ballFire.transform);
    }

    // EVENT FUNCTIONS
    public void HandleUpgrade1()
    {
        if (m_money > m_fireRateCost && m_fireRateLevel < m_upgradeMaxLevel)
        {
            m_money -= m_fireRateCost;
            m_fireRateLevel++;
            m_fireRateCost += 100;
        }

        CheckMoney();
        UpdateUpgradeValues();
        GameEvents.current.MoneyChange();
        GameEvents.current.UpgradesChange(1, m_fireRateLevel, m_fireRateCost);
        PlayerPrefs.SetInt("FireRateLevel", m_fireRateLevel);
        PlayerPrefs.SetInt("FireRateCost", m_fireRateCost);
        PlayerPrefs.SetInt("Money", m_money);
    }

    public void HandleUpgrade2()
    {
        if (m_money > m_ballBounceCost && m_ballBounceLevel < 5)
        {
            m_money -= m_ballBounceCost;
            m_ballBounceLevel++;
            m_ballBounceCost += 1000;
        }

        CheckMoney();
        UpdateUpgradeValues();
        GameEvents.current.MoneyChange();
        GameEvents.current.UpgradesChange(2, m_ballBounceLevel, m_ballBounceCost);
        PlayerPrefs.SetInt("BallBounceLevel", m_ballBounceLevel);
        PlayerPrefs.SetInt("BallBounceCost", m_ballBounceCost);
        PlayerPrefs.SetInt("Money", m_money);
    }

    public void HandleUpgrade3()
    {
        if (m_money > m_incomeCost && m_incomeLevel < m_upgradeMaxLevel)
        {
            m_money -= m_incomeCost;
            m_incomeLevel++;
            m_incomeCost += 100;
        }

        CheckMoney();
        UpdateUpgradeValues();
        GameEvents.current.MoneyChange();
        GameEvents.current.UpgradesChange(3, m_incomeLevel, m_incomeCost);
        PlayerPrefs.SetInt("IncomeLevel", m_incomeLevel);
        PlayerPrefs.SetInt("IncomeCost", m_incomeCost);
        PlayerPrefs.SetInt("Money", m_money);
    }

    public void HandleVictory(float multiplier)
    {
        m_themeSet = false;
        ResetGame();
        m_enemyPooler.AddSpawnProbability(Mathf.InverseLerp(1,300, m_level)); //Not sure if we want to use 300 as max level...
        m_level++;
        PlayerPrefs.SetInt("Level", m_level);
        CalculateDifficulty(m_level);
        GameEvents.current.HandleVictory(m_level, (int)(m_earned * multiplier));
    }

    void RewardVictory(int mult)
    {
        HandleTheme();
        m_money += m_earned * mult;
        m_earned = 0;
        PlayerPrefs.SetInt("Money", m_money);
        GameEvents.current.MoneyChange();
        CheckMoney();
    }

    public void HandleDefeat()
    {
        //ResetGame();
        m_playerDead = true;
        m_playerAnimator.SetTrigger("Hit");
        m_player.transform.localEulerAngles = new Vector3(-90, 0, 0);
        var playerLaunchDestination = new Vector3(1, 0, Random.Range(-1f, 0f)) * 10;
        m_player.transform.DOMove(new Vector3(0, .8f) + m_player.transform.localPosition + playerLaunchDestination, 3)
            .OnComplete(() => GameEvents.current.HandleDefeat());
    }

    void SkipLevel()
    {
        m_level++;
        HandleTheme();
        PlayerPrefs.SetInt("Level", m_level);
        GameEvents.current.LevelChange(m_level);
        CalculateDifficulty(m_level);
    }

    void HandleReward()
    {
        m_earned += Random.Range(1, m_income + 5);
    }

    void SpawnInitialEnemies()
    {
        foreach (var t in m_enemySpawnPositions)
        {
            var enemy = m_enemyPooler.SpawnEnemy(2);
            enemy.transform.rotation = Quaternion.identity;
            enemy.transform.position = t.position;
        }
    }

    public void ResetGame()
    {
        m_playerDead = false;
        if (m_startedSpawning != null) StopCoroutine(m_startedSpawning);
        m_startedSpawning = null;
        m_playGame = false;
        m_isBossFight = false;
        Enemy.isPassive = true;
        m_allyCount = 0;
        m_playerAnimator.SetBool("Swing", false);
        EnemyClass.ResetStatics();
        GameEvents.current.DeleteAnimation();
        m_player.transform.localPosition = new Vector3(125.4778f, 1.418f, -88.17085f);
        RemoveAllCharacters();
        SpawnInitialEnemies();
    }

    void RemoveAllCharacters()
    {
        if (m_shield.activeSelf) m_shield.GetComponent<Shield>().DestroyShield();
        if (m_bossInstance) Destroy(m_bossInstance);
        var e = GameObject.FindGameObjectsWithTag("Enemy");
        e = e.Concat(GameObject.FindGameObjectsWithTag("Ally")).ToArray();
        foreach (var i in e) Destroy(i);
    }

    void HandleCosmetic(int type, int i, int cost)
    {
        if (cost <= m_money)
        {
            m_money -= cost;
            PlayerPrefs.SetInt("Money", m_money);
            GameEvents.current.MoneyChange();
            GameEvents.current.AnswerRequest(true, type, i);
            m_cosmetics[type, i, 0] = 1;
        }
        else
        {
            GameEvents.current.AnswerRequest(false, type, i);
        }
    }

    void HandleCosmeticState(int type, int i, int state)
    {
        m_cosmetics[type, i, 1] = state;
    }

    public void CheckMoney()
    {
        var disable1 = false;
        var disable2 = false;
        var disable3 = false;
        if (m_money < m_fireRateCost) disable1 = true;

        if (m_money < m_ballBounceCost) disable2 = true;

        if (m_money < m_incomeCost) disable3 = true;

        GameEvents.current.CheckAfford(disable1, disable2, disable3);
    }

    void LoadTrail(Gradient trail)
    {
        m_currTrail = trail;
    }

    void SetPlayerMaterial(Material mat)
    {
        m_defaultMat = mat;
    }

    void SendCosmeticStates()
    {
        GameEvents.current.SendCosmeticStates(m_cosmetics);
    }

    void HandleTheme()
    {
        if (m_level % 2 == 0 && !m_themeSet)
        {
            m_themeSet = true;
            int n;
            do
            {
                n = Random.Range(0, 4);
            } while (n == m_currTheme);

            m_currTheme = n;
            GameEvents.current.SetTheme(n);
        }
    }

    #region Cheat Methods
    public void SetLevel(int value)
    {
        m_level = value;
        CalculateDifficulty(value);
        GameEvents.current.LevelChange(value);
    }
    #endregion
}