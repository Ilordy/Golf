using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanBannerController : MonoBehaviour
{
    private bool m_triggered;

    void Start()
    {
        //Wait one frame to allow the water to be correctly positioned.
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, int.MaxValue, ~0, QueryTriggerInteraction.Collide))
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
    }

    void Update()
    {
        if (Manager.I.BossInstance && !m_triggered && Manager.I.BossInstance.transform.position.z > transform.position.z)
        {
            m_triggered = true;
            WorldManager.I.DisplayMultiplierText();
        }
    }
}