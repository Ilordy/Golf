using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldManager : Singleton<WorldManager>
{
    [SerializeField] GameObject oceanPrefab;
    [SerializeField] float bannerStartPos;
    [SerializeField] float offsetBetweenBanners;
    float bannerMultiplierAmnt = 1.0f;
    void Start()
    {

    }

    void Update()
    {

    }

    public void SpawnOceans(Vector3 bossLandingPoint)
    {
        Debug.Log(bossLandingPoint);
        //-x is our forward direction for this game.
        //gotta get the absolute value of x first and then add it by 50% of it's area for bigger room of error.
        float finalPos = Mathf.Abs(bossLandingPoint.x);
        for (float i = Mathf.Abs(bannerStartPos); i < finalPos + (finalPos * .50f); i += offsetBetweenBanners)
        {
            GameObject banner = Instantiate(oceanPrefab, new Vector3(i * -1, 0, 0), Quaternion.identity, transform);
            banner.GetComponentInChildren<TextMeshProUGUI>().text = "x" + bannerMultiplierAmnt.ToString("F1");
            bannerMultiplierAmnt += 0.2f;
        }
    }

    protected override void InternalInit()
    {

    }

    protected override void InternalOnDestroy()
    {

    }
}
