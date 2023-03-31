using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MobileTools;
public class WorldManager : Singleton<WorldManager>
{
    [SerializeField] GameObject oceanPrefab;
    [SerializeField] float bannerStartPos;
    [SerializeField] float offsetBetweenBanners;
    /// <summary>
    /// Objects to hide when boss gets launched.
    /// </summary>
    [SerializeField] GameObject[] m_objectsToHide;
    private List<GameObject> banners = new List<GameObject>();
    private FloatingTextPooler m_pooler;
    float totalMultAmnt = 1.0f, currentMultAmnt = 1.0f;
    void Start()
    {
        m_pooler = GetComponent<FloatingTextPooler>();
    }

    public void SpawnOceans(Vector3 bossLandingPoint)
    {
        //-x is our forward direction for this game.
        //gotta get the absolute value of x first and then add it by 50% of it's area for bigger room of error.
        float finalPos = Mathf.Abs(bossLandingPoint.z);
        ToggleObstacles(false);
        for (float i = Mathf.Abs(bannerStartPos); i < finalPos + (finalPos * .50f); i += offsetBetweenBanners)
        {
            GameObject banner = Instantiate(oceanPrefab, new Vector3(bossLandingPoint.x - 30, 70, i), Quaternion.Euler(0, 180, 0), transform);
            banners.Add(banner);
            banner.GetComponentInChildren<TextMeshProUGUI>().text = "x" + totalMultAmnt.ToString("F1");
            totalMultAmnt += 0.2f;
        }
    }

    public void DisplayMultiplierText()
    {
        m_pooler.CreateText("<sprite=0>x" + currentMultAmnt.ToString("F1"));
        currentMultAmnt += 0.2f;
    }

    public void BossHitOcean()
    {
        Manager.I.HandleVictory(currentMultAmnt);
    }

    public void ResetBanners()
    {
        foreach (GameObject banner in banners)
        {
            Destroy(banner);
        }
        ToggleObstacles(true);
        banners.Clear();
        currentMultAmnt = 1;
        totalMultAmnt = 1;
    }

    void ToggleObstacles(bool active)
    {
        foreach (GameObject obj in m_objectsToHide)
            obj.SetActive(active);
    }
}
