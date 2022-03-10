using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cosmetic")]
public class Cosmetic : ScriptableObject {
    //GENERAL
    public int Type = 0;
    public int Index = 0;
    public int Cost = 1000;
    public Vector3 Offset;
    public Vector3 Rotation;
    public GameObject Model;
    public Material PlayerDefaultMaterial;
}
