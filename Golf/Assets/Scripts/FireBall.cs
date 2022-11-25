using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private float _sliderForce;

    public float SliderForce { get => _sliderForce; set => _sliderForce = value; }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Boss"))
        {
            other.gameObject.GetComponent<BossEnemy>().AddRagDollForce(_sliderForce);
        }
    }

}
