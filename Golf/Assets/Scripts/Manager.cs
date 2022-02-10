using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Manager : MonoBehaviour
{

    public GameObject enemy;
    public GameObject projectile;
    public GameObject spawner;
    public Slider powerUpSlider;


    GameObject[] enemies;
    GameObject[] powerUpProjectiles;

    [HideInInspector]
    public int enemiesKilled = 0;
    public int powerUpReq = 5;

    float enemySpawnTimer = 0f;
    float enemySpawnInterval = 1f;

    float fireRateCounter = 0f;
    float fireRate = 0.2f;

    void Start()
    {
        enemySpawnTimer = enemySpawnInterval;
        powerUpSlider.maxValue = powerUpReq;
        powerUpSlider.value = 0;
    }

    void Update()
    {
        // Spawn Enemies
        enemySpawnTimer -= Time.deltaTime;
        if (enemySpawnTimer <= 0) {
            enemySpawnTimer = enemySpawnInterval;
            Instantiate(enemy, new Vector3(-20+Random.Range(-10,10),2,Random.Range(-7,7)), Quaternion.identity);
        }

        if(Input.GetMouseButtonDown(0)) {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            powerUpProjectiles = new GameObject[enemies.Length];
            for (int i = 0; i <= enemies.Length-1; i++) {
                int r = 3;
                float x = r * Mathf.Cos(i * (Mathf.PI / (enemies.Length-1)));
                float y = r * Mathf.Sin(i * (Mathf.PI / (enemies.Length-1)));
                powerUpProjectiles[i] = Instantiate(projectile, spawner.transform.position + new Vector3(0,y,x), Quaternion.identity);
            }
        }
        if(Input.GetMouseButtonUp(0)) {
            for (int i = 0; i < enemies.Length; i++) {
                powerUpProjectiles[i].transform.LookAt(enemies[i].transform.position);
                powerUpProjectiles[i].GetComponent<Rigidbody>().useGravity = true;
                powerUpProjectiles[i].GetComponent<Rigidbody>().velocity = powerUpProjectiles[i].transform.forward * 50f;
            }
        }

        //allowFire(0);

        // if (powerUpSlider.value < powerUpReq) {
        //     allowFire(0);
        // } else if(powerUpSlider.value >= powerUpReq && enemySpawnTimer2 > 0) {
        //     enemySpawnTimer2 -= Time.deltaTime;
        //     allowFire(1);
        // } else {
        //     enemySpawnTimer2 = powerUpDuration;
        //     enemiesKilled = 0;
        //     powerUpSlider.value = 0;
        // }
        // powerUpSlider.value = enemiesKilled;
    }

    void allowFire(int mode) {
        if (mode == 0) {
            if (Input.GetMouseButtonDown(0)) {
                fireProjectile(ref fireRateCounter, fireRate);
            }
        }
    }

    void fireProjectile(ref float fireRateCounter, float nextFire) {
        if (Time.time > fireRateCounter) {
            fireRateCounter = Time.time + nextFire;
            Vector3 flatAimTarget = calculateTarget();
            GameObject p = Instantiate(projectile, spawner.transform.position, Quaternion.identity);
            p.transform.LookAt(flatAimTarget);
        }
    }

    Vector3 calculateTarget() {
        Vector3 cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        Vector3 screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return screenPoint + cursorRay / Mathf.Abs(cursorRay.y) * Mathf.Abs(screenPoint.y - spawner.transform.position.y);
    }
}