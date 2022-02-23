using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpAnimation : MonoBehaviour {
    public GameObject animProjectile;
    public GameObject player;

    List<GameObject> spawns = new List<GameObject>();
    int spawned = 0;
    float c = 0;

    public void AnimatePowerUp(int enemiesLength, float pot) {
        if (!gameObject.GetComponent<AudioSource>().isPlaying) {
            gameObject.GetComponent<AudioSource>().Play();
        }
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
            float x = Mathf.Lerp(gameObject.transform.position.x,player.transform.position.x,factor);
            float y = Mathf.Lerp(gameObject.transform.position.y,player.transform.position.y + targetZ,factor);
            float z = Mathf.Lerp(gameObject.transform.position.z,player.transform.position.z + targetY,factor);

            spawns[i].transform.position = new Vector3(x,y,z);
        }
    }

    public void DeleteAnimation() {
        gameObject.GetComponent<AudioSource>().Stop();
        spawned = 0;
        for (int i = 0; i < spawns.Count; i++) {
            Destroy(spawns[i]);
        }
    }
}
