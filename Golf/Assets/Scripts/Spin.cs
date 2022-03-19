using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {
    [SerializeField] Vector3 rotation;
    [SerializeField] int multiplier = 5;
    
    void Update() {
        transform.GetChild(0).transform.Rotate(rotation * multiplier);
    }
}
