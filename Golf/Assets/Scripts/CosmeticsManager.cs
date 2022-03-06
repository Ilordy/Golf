using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticsManager : MonoBehaviour {
    [SerializeField] GameObject player;

    [SerializeField] CosmeticItem[,] cosmetics = new CosmeticItem[3,5]; 

    GameObject[] equipped = new GameObject[3];
    CosmeticItem[] equippedScriptReference = new CosmeticItem[3];

    void Start() {
        GameEvents.current.OnGetCosmetics += GetCosmetics;
        GameEvents.current.OnPurchaseCosmetic += Purchase;
        GameEvents.current.OnEquipCosmetic += Equip;
    }

    void Purchase(int type, int i) {
        Equip(type, i);
    }

    void Equip(int type, int i) {
        if (equipped[type] != null) {
            if (equipped[type].name == cosmetics[type,i].model.name + "(Clone)") {
                Destroy(equipped[type]);
                cosmetics[type,i].SetEquip();
                equippedScriptReference[type] = null;
                return;
            }
        }

        if (equippedScriptReference[type] != null) {
            equippedScriptReference[type].SetEquip();
        }

        Destroy(equipped[type]);
        
        cosmetics[type,i].SetEquip();
        GameObject instance = Instantiate(cosmetics[type,i].model, Vector3.zero, Quaternion.identity);
        instance.transform.parent = player.transform;
        instance.transform.position = player.transform.position;
        equipped[type] = instance;
        equippedScriptReference[type] = cosmetics[type,i];
    }

    void GetCosmetics(int type) {
        CosmeticItem[] cosmeticsInScene = GameObject.FindObjectsOfType<CosmeticItem>();
        foreach (CosmeticItem cosmetic in cosmeticsInScene) {
            if (cosmetic.type == type) {
                cosmetics[type, cosmetic.index] = cosmetic;
            }
        }
    }
}