using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpAnimation : MonoBehaviour {
    public GameObject animProjectile;
    public Manager GameManager;
    public GameObject player;
    public Transform spawner;

    List<GameObject> spawns = new List<GameObject>();
    Animator animator;
    int spawned = 0;
    float c = 0;

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void AnimatePowerUp(int enemiesLength, float pot) {
        animator.SetBool("Animate", true);
        GameEvents.current.ChargeStart();
        c += Time.deltaTime*2;
        if (spawned < enemiesLength) {
            spawns.Add(Instantiate(animProjectile,Vector3.zero,Quaternion.identity));
            spawned++;
        }

        for (int i = 0; i < spawns.Count; i++) {
            int r = 3;
            float targetY = r * Mathf.Cos(Mathf.PI/2.5f + ((Mathf.PI/2 * i) / enemiesLength/(Mathf.PI/2)));
            float targetZ = r * Mathf.Sin(Mathf.PI/2.5f + ((Mathf.PI/2 * i) / enemiesLength/(Mathf.PI/2)));
            float factor = c - (i/pot)/enemiesLength;
            float x = Mathf.Lerp(spawner.position.x,player.transform.position.x,factor);
            float y = Mathf.Lerp(spawner.position.y,player.transform.position.y + targetZ,factor);
            float z = Mathf.Lerp(spawner.position.z,player.transform.position.z + targetY,factor);

            spawns[i].transform.position = new Vector3(x,y,z);
        }
        
        if (c > 1) animator.SetBool("Animate", false);
    }

    public void DeleteAnimation() {
        GameEvents.current.ChargeStop();
        animator.SetBool("Animate", false);
        spawned = 0;
        c = 0;
        for (int i = 0; i < spawns.Count; i++) {
            Destroy(spawns[i]);
        }
        spawns = new List<GameObject>();
    }
}
