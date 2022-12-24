using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMultiplierTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        WorldManager.I.DisplayMultiplierText();
    }
}
