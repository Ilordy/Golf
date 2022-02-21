using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragSelfDestruct : MonoBehaviour
{
    void Update()
    {
        if (gameObject.transform.parent == null) {
            Destroy(gameObject,3);
        }
    }
}
