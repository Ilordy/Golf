using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {

    UIManager UIManager;
    GameObject hatShop, skinShop, trailShop;
    Button hatsButton, skinsButton, trailsButton, mainBackButton;
    Button hatBackButton, skinBackButton, trailBackButton;
    public Button[] hatsPurchaseButtons;
    Stack<Transform> menuStack;

    void Awake() {
        UIManager = transform.parent.GetComponent<UIManager>();

        //Cache Main Shop Elements
        hatsButton = transform.GetChild(2).transform.GetChild(0).GetComponent<Button>();
        skinsButton = transform.GetChild(2).transform.GetChild(1).GetComponent<Button>();
        trailsButton = transform.GetChild(2).transform.GetChild(2).GetComponent<Button>();
        mainBackButton = transform.GetChild(3).GetComponent<Button>();

        //Cache Hats Shop Elements
        hatShop = transform.parent.GetChild(3).gameObject;
        hatBackButton = hatShop.transform.GetChild(2).GetComponent<Button>();

        //Cache Skins Shop Elements
        skinShop = transform.parent.GetChild(4).gameObject;
        skinBackButton = skinShop.transform.GetChild(2).GetComponent<Button>();

        //Cache Trails Shop Elements
        trailShop = transform.parent.GetChild(5).gameObject;
        trailBackButton = trailShop.transform.GetChild(2).GetComponent<Button>();

        //Add Listeners to Main Shop Elements
        hatsButton.onClick.AddListener(()=>{gameObject.SetActive(false); hatShop.SetActive(true); menuStack.Push(hatShop.transform);});
        skinsButton.onClick.AddListener(()=>{gameObject.SetActive(false); skinShop.SetActive(true); menuStack.Push(skinShop.transform);});
        trailsButton.onClick.AddListener(()=>{gameObject.SetActive(false); trailShop.SetActive(true); menuStack.Push(trailShop.transform);});
        mainBackButton.onClick.AddListener(Back);

        //Add Listeners to Hats Shop Elements
        hatBackButton.onClick.AddListener(Back);

        //Add Listeners to Skins Shop Elements
        skinBackButton.onClick.AddListener(Back);

        //Add Listeners to Trails Shop Elements
        trailBackButton.onClick.AddListener(Back);

        foreach (Button button in hatsPurchaseButtons) {
            button.onClick.AddListener(() => {});
        }
    }

    void OnEnable() {
        menuStack = UIManager.MenuStack;
    }

    void Back() {
        menuStack.Peek().gameObject.SetActive(false);
        menuStack.Pop();
        menuStack.Peek().gameObject.SetActive(true);
    }
}
