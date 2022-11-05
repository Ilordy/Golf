using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MK.Toon;

public class ColorManager : MonoBehaviour {

    [SerializeField] Material runway1, runway2, rails;

    [SerializeField] Color[] theme1, theme2, theme3, theme4;
    
    Color[][] Themes;

    void Start() {
        Themes = new Color[4][] {theme1, theme2, theme3, theme4};
        GameEvents.current.OnSetTheme += SetTheme;
    }

    void SetTheme(int n) {
        Properties.albedoColor.SetValue(rails, Themes[n][0]);  
        runway1.color = Themes[n][1];
        runway2.color = Themes[n][2];
    }
}
