using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticsManager : MonoBehaviour {
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] hats = new GameObject[5];
    int[] hatsState = new int[5];
    GameObject equippedHat;

    void Start() {
        GameEvents.current.OnEquipCosmetic += EquipCosmetic;
        GameEvents.current.OnUnequipCosmetic += UnequipCosmetic;
    }

    void EquipCosmetic(int i) {
        if (hatsState[i] == 1) {
            Debug.Log("Cosmetic already equipped");
        }
        if (hatsState[i] == -1) {
            Debug.Log("Cosmetic not purchased");
        }
        hatsState[i] = 1;
        GameObject instance = Instantiate(hats[i], player.transform.position, Quaternion.identity);
        instance.transform.parent = player.transform;
        equippedHat = instance;
    }

    void UnequipCosmetic(int i) {
        if (hatsState[i] == 0) {
            Debug.Log("Cosmetic not currently equipped");
        }
        if (hatsState[i] == -1) {
            Debug.Log("Cosmetic not purchased");
        }
        hatsState[i] = 0;
        Destroy(equippedHat);
    }
}