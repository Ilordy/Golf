using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CosmeticItem : MonoBehaviour {
    public Cosmetic data;

    int index;
    int type;
    int cost;
    GameObject model;

    GameObject player;
    GameObject cosmeticInstance;

    Material defaultMat;
    Material thisMat;

    Gradient thisTrail;

    Manager GameManager;
    Button purchaseButton;
    TextMeshProUGUI purchaseButtonText;

    bool purchased = false;
    bool equipped = false;

    void Awake() {
        // Set object data from scriptable object
        type = data.Type;
        cost = data.Cost;
        model = data.Model;
        index = data.Index;

        // Get relevant references
        player = GameObject.Find("Player");
        GameManager = GameObject.Find("Manager").GetComponent<Manager>();
        purchaseButton = transform.parent.parent.GetChild(2).GetComponent<Button>();
        purchaseButtonText = purchaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (type == 1) {
            defaultMat = data.PlayerDefaultMaterial;
            thisMat = model.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        } else if (type == 2) {
            thisTrail = model.GetComponent<TrailRenderer>().colorGradient;
        }

        // Instantiate UI showcase
        GameObject instance = Instantiate(model, Vector3.zero, Quaternion.identity);
        instance.layer = 5;
        instance.transform.parent = gameObject.transform;
        instance.transform.position = transform.position;

        if (type == 1) instance.transform.position -= new Vector3(0,0.5f,0);

        GameEvents.current.OnAnswerRequest += HandleRequestAnswer;
        GameEvents.current.OnUnequipOthers += UnequipOthers;
        GameEvents.current.OnSendCosmeticStates += SetStates;

        GameEvents.current.RequestcosmeticStates();
    }

    void Start() {
        purchaseButton.onClick.AddListener(Purchase);
    }

    void OnEnable() {
        if (!purchased) {
            purchaseButtonText.text = cost.ToString();
        } else {
            if (equipped) {
                purchaseButtonText.text = "Unequip";
            } else {
                purchaseButtonText.text = "Equip";
            }
        }

        if (!purchased && !CanBuy()) {
            purchaseButton.interactable = false;
        } else {
            purchaseButton.interactable = true;
        }
    }

    bool CanBuy() {
        if (GameManager.Money >= cost) return true; return false;
    }

    void Purchase() {
        if (!purchased) {
            GameEvents.current.RequestCosmetic(type, index, cost);
        } else if (equipped) {
            Unequip(type);
        } else {
            Equip(type);
        }
    }

    void HandleRequestAnswer(bool answer, int t, int i) {
        if (t == type && i == index && answer) {
            Equip(type);
            purchased = true;
        }
    }

    void Equip(int t) {
        SetEquip();
        GameEvents.current.SetEquip(type, index, 1);
        GameEvents.current.UnequipOthers(type, index);
        if (t == 0) {
            cosmeticInstance = Instantiate(model, Vector3.zero, Quaternion.identity);
            cosmeticInstance.transform.parent = player.transform;
            cosmeticInstance.transform.position = player.transform.position;
        } else if (t == 1) {
            player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = thisMat;
            GameEvents.current.SetStartMaterial(thisMat);
        } else if (t == 2) {
            GameEvents.current.LoadTrail(thisTrail);
        }
    }

    void Unequip(int t) {
        SetEquip();
        GameEvents.current.SetEquip(type, index, 0);
        if (t == 0) {
            Destroy(cosmeticInstance);
        } else if (t == 1) {
            player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = defaultMat;
            GameEvents.current.SetStartMaterial(defaultMat);
        } else if (t == 2) {
            GameEvents.current.UnloadTrail();
        }
        
    }

    void UnequipOthers(int t, int i) {
        if (t == type && i != index && equipped) {
            Unequip(type);
        }
    }

    void SetEquip() {
        equipped = !equipped;
        string action = equipped ? "Unequip" : "Equip";
        purchaseButtonText.text = action.ToString();
    }

    void SetStates(int[,,] cosmetics) {
        if (cosmetics[type,index,0] < 1) {
            purchased = false;
        } else {
            purchased = true;
        }
        if (cosmetics[type,index,1] < 1) {
            equipped = false;
        } else {
            equipped = true;
        }
    }
}
