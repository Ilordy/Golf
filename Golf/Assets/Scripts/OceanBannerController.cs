using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanBannerController : MonoBehaviour
{
    private bool m_triggered;
    void Start()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, int.MaxValue))
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.I.BossInstance && !m_triggered && Manager.I.BossInstance.transform.position.z > transform.position.z)
        {
            m_triggered = true;
            WorldManager.I.DisplayMultiplierText();
        }
    }
}
