using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CosmeticItem : MonoBehaviour {
    public Cosmetic data;

    string type;
    int cost;
    GameObject model;

    Manager GameManager;
    Button purchaseButton;
    TextMeshProUGUI purchaseButtonText;

    bool purchased = false;
    bool equipped = false;

    void Awake() {
        type = data.Type;
        cost = data.Cost;
        model = data.Model;

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
        if (CanBuy()) {
            purchaseButton.interactable = true;
        } else {
            purchaseButton.interactable = false;
        }
    }

    bool CanBuy() {
        if (GameManager.Money > cost) return true; return false;
    }

    void Purchase() {
        if (CanBuy()) {
            purchased = true;
            equipped = true;
        }
    }
}
