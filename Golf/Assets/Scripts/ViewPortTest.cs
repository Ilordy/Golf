using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPortTest : MonoBehaviour
{
    [Range(0,1)]
    public float xPos;
    public float zPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(xPos,1,zPos));
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(p.x,1.8f,p.z), 1F);
    }
}
