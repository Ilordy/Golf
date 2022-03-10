using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {
    public Vector3 rotation;
    public int multiplier = 5;
    
    void Update() {
        transform.GetChild(0).transform.Rotate(rotation * multiplier);
    }
}
