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

    Manager GameManager;
    Button purchaseButton;
    TextMeshProUGUI purchaseButtonText;

    bool purchased = false;
    bool equipped = false;

    void Awake() {
        type = data.Type;
        cost = data.Cost;
        model = data.Model;
        index = data.Index;

        player = GameObject.Find("Player");
        GameManager = GameObject.Find("Manager").GetComponent<Manager>();
        purchaseButton = transform.parent.parent.GetChild(2).GetComponent<Button>();
        purchaseButtonText = purchaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        GameObject instance = Instantiate(model, Vector3.zero, Quaternion.identity);
        instance.layer = 5;
        instance.transform.parent = gameObject.transform;
        instance.transform.position = transform.position;

        GameEvents.current.OnPurchaseCosmetic += HandlePurchase;
        GameEvents.current.OnUnequipOthers += UnequipOthers;
    }

    void Start() {
        purchaseButtonText.text = cost.ToString();

        purchaseButton.onClick.AddListener(Purchase);
    }

    void OnEnable() {
        purchaseButton.interactable = false;
        if (!purchased && CanBuy()) {
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
            Unequip();
        } else {
            Equip();
        }
    }

    void HandlePurchase(bool answer, int t, int i) {
        if (t == type && i == index && answer) {
            Equip();
            purchased = true;
        }
    }

    void Equip() {
        SetEquip();
        GameEvents.current.SetEquip(type, index, 1);
        GameEvents.current.UnequipOthers(type, index);
        cosmeticInstance = Instantiate(model, Vector3.zero, Quaternion.identity);
        cosmeticInstance.transform.parent = player.transform;
        cosmeticInstance.transform.position = player.transform.position;
    }

    void Unequip() {
        SetEquip();
        GameEvents.current.SetEquip(type, index, 0);
        Destroy(cosmeticInstance);
    }

    void UnequipOthers(int t, int i) {
        if (t == type && i != index && equipped) {
            Unequip();
        }
    }

    void SetEquip() {
        equipped = !equipped;
        string action = equipped ? "Unequip" : "Equip";
        purchaseButtonText.text = action.ToString();
    }
}
