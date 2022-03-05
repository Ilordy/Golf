using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cosmetic")]
public class Cosmetic : ScriptableObject {
    public string Type = "none";
    public int Cost = 1000;
    public GameObject Model;
}
