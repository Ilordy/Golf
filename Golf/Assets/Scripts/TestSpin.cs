using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpin : MonoBehaviour {
    public int multiplier = 5;
    void Update() {
        transform.GetChild(0).transform.Rotate(Vector3.one * multiplier);
    }
}
