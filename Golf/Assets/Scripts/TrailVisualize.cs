using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrailVisualize : MonoBehaviour {
    // USED TO VISUALIZE TRAIL COLORS IN THE UI
    LineRenderer line;

    void Start() {
        line = GetComponent<LineRenderer>();
    }
    void Update() {

        for (int i = 0; i < 30; i++) {
            float angle = (Mathf.PI*2)/30 * i;
            float x = 2 * Mathf.Sin(angle);
            float y = 2 * Mathf.Cos(angle);
            line.SetPosition(i,new Vector3(x,y,0));
        }
    }
}
