using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CosmeticItem : MonoBehaviour {
    public Cosmetic data;
    public int index;
    public int type;
    int cost;
    public GameObject model;

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

        GameManager = GameObject.Find("Manager").GetComponent<Manager>();
        purchaseButton = transform.parent.parent.GetChild(2).GetComponent<Button>();
        purchaseButtonText = purchaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        GameObject instance = Instantiate(model, Vector3.zero, Quaternion.identity);
        instance.layer = 5;
        instance.transform.parent = gameObject.transform;
        instance.transform.position = transform.position;
    }

    void Start() {
        purchaseButtonText.text = cost.ToString();

        purchaseButton.onClick.AddListener(Purchase);
    }

    void OnEnable() {
        purchaseButton.interactable = true;
        if (!purchased && !CanBuy()) {
            purchaseButton.interactable = false;
        }
    }

    bool CanBuy() {
        if (GameManager.Money >= cost) return true; return false;
    }

    void Purchase() {
        if (!purchased) {
            if (CanBuy()) {
                purchased = true;
                GameEvents.current.PurchaseCosmetic(type, index);
            }
        } else {
            GameEvents.current.EquipCosmetic(type,index);
        }
    }

    public void SetEquip() {
        equipped = !equipped;
        string action = equipped ? "Unequip" : "Equip";
        purchaseButtonText.text = action.ToString();
    }
}
