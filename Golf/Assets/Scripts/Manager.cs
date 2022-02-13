using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Manager : MonoBehaviour {

    public GameObject enemy;
    public GameObject projectile;
    public GameObject powerUpProjectile;
    public GameObject spawner;
    public Slider powerUpSlider;


    GameObject[] enemies;
    GameObject[] powerUpProjectiles;

    [HideInInspector]
    public int enemiesKilled = 0;
    [HideInInspector]
    public int enemiesAlive = 0;

    private bool    powerUpEnabled = false;
    private float   powerUpTimer = 0f;
    public float    powerUpChargeTime = 1f;
    public int      powerUpReq = 5;

    float enemySpawnTimer = 0f;
    float enemySpawnInterval = 1f;

    float pot = 0f;

    float fireRateCounter = 0f;
    float fireRate = 0.5f;

    float ballSpeed = 25f;

    public int fireRateLevel = 1;
    public int ballSpeedLevel = 1;
    int upgradeMaxLevel = 20;

    void Start() {
        enemySpawnTimer = enemySpawnInterval;
        powerUpSlider.maxValue = powerUpReq;
        powerUpSlider.value = 0;
    }

    void Update() {
        // Handle Fire Rate Upgrade
        fireRate = Mathf.Lerp(0.5f,0.1f,fireRateLevel/upgradeMaxLevel);

        // Handle Ball Speed Upgrade
        ballSpeed = Mathf.Lerp(25f,50f,ballSpeedLevel/upgradeMaxLevel);

        // Spawn Enemies
        enemySpawnTimer -= Time.deltaTime;
        if (enemySpawnTimer <= 0) {
            enemySpawnTimer = enemySpawnInterval;
            Instantiate(enemy, new Vector3(-20+Random.Range(-10,10),2,Random.Range(-7,7)), Quaternion.identity);
            enemiesAlive++;
        }

        if (enemiesKilled <= powerUpReq) {
            powerUpSlider.value = enemiesKilled;
        }

        if (Input.GetMouseButtonDown(0)) {
            powerUpTimer = Time.time;
        }

        if (Input.GetMouseButton(0)) {
            float holdTime = Time.time - powerUpTimer;
            if (enemiesKilled >= powerUpReq && holdTime > 0.1f) {
                pot += Time.deltaTime;
                powerUpSlider.value = Mathf.Lerp(powerUpReq, 0, pot / 1f);
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            float holdTime = Time.time - powerUpTimer;
            if (holdTime >= 1f && enemiesKilled >= powerUpReq) {
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
                enemiesKilled = 0;
                pot = 0;
            } else {
                fireProjectile(ref fireRateCounter, fireRate);
                pot = 0;
                powerUpSlider.value = enemiesKilled;
            }
        }
    }

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
}

        // powerUpSlider.value = enemiesKilled;
        //     if (Input.GetMouseButton(0)) {
        //         if (powerUpTimer >= 1.5f && powerUpTimer <= 1.6f) {
        //             enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //             powerUpProjectiles = new GameObject[enemies.Length];
        //             for (int i = 0; i <= enemies.Length - 1; i++) {
        //                 int r = 3;
        //                 float x = r * Mathf.Cos(i * (Mathf.PI / (enemies.Length - 1)));
        //                 float y = r * Mathf.Sin(i * (Mathf.PI / (enemies.Length - 1)));
        //                 powerUpProjectiles[i] = Instantiate(projectile, spawner.transform.position + new Vector3(0, y, x), Quaternion.identity);
        //                 powerUpProjectiles[i].GetComponent<Rigidbody>().useGravity = false;
        //                 powerUpProjectiles[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
        //             }
        //         }
        //         powerUpTimer += 0.1f;
        //     }
        //     if (Input.GetMouseButtonUp(0)) {
        //         if (powerUpTimer >= 5f) {
        //             for (int i = 0; i < enemies.Length; i++) {
        //                 powerUpProjectiles[i].transform.LookAt(enemies[i].transform.position);
        //                 powerUpProjectiles[i].GetComponent<Rigidbody>().useGravity = true;
        //                 powerUpProjectiles[i].GetComponent<Rigidbody>().velocity = powerUpProjectiles[i].transform.forward * 50f;
        //             }
        //             powerUpTimer = 0f;
        //             enemiesKilled = 0;
        //         }
        //         else {
        //             fireProjectile(ref fireRateCounter, fireRate);
        //             powerUpTimer = 0f;
        //         }
        //     }


        // SECOND MORE UP TO DATE ITERATION OF SHOOTING LOGIC


        // if (Input.GetMouseButton(0)) {
        //     powerUpTimer += Time.deltaTime;
        //     if (powerUpEnabled == true && powerUpTimer > 0.2f) {
        //         pot += Time.deltaTime;
        //         powerUpSlider.value = Mathf.Lerp(powerUpReq, 0, pot / 1f);
        //     }
        // }
        // if (Input.GetMouseButtonUp(0)) {
        //     if (enemiesKilled >= powerUpReq && powerUpTimer >= powerUpChargeTime) {
        //         enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //         powerUpProjectiles = new GameObject[enemies.Length];
        //         for (int i = 0; i <= enemies.Length - 1; i++) {
        //             int r = 3;
        //             float x = r * Mathf.Cos(i * (Mathf.PI / (enemies.Length - 1)));
        //             float y = r * Mathf.Sin(i * (Mathf.PI / (enemies.Length - 1)));
        //             powerUpProjectiles[i] = Instantiate(powerUpProjectile, spawner.transform.position + new Vector3(0, y, x), Quaternion.identity);
        //             powerUpProjectiles[i].GetComponent<Rigidbody>().useGravity = true;
        //             powerUpProjectiles[i].transform.LookAt(enemies[i].transform.position);
        //             powerUpProjectiles[i].GetComponent<Rigidbody>().velocity = powerUpProjectiles[i].transform.forward * 100f;
        //         }
        //         enemiesKilled = 0;
        //         powerUpTimer = 0f;
        //         powerUpEnabled = false;
        //         pot = 0;
        //     } else {
        //         fireProjectile(ref fireRateCounter, fireRate);
        //         powerUpTimer = 0f;
        //         powerUpSlider.value = enemiesKilled;
        //         pot = 0;
        //     }
        // }