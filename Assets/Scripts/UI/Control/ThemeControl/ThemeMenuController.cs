using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeMenuController : MonoBehaviour
{

    public Button LightButton, DarkButton;

    public GameObject LightTick, DarkTick;
    public ColourSwatchManager ColourSwatchManager;


    private void Awake() 
    {
        InitTheme();
    }

    private void Start() 
    {
        InitTheme();

        LightButton.onClick.AddListener(delegate{SetTheme(0);});
        DarkButton.onClick.AddListener(delegate{SetTheme(1);});
    }


    private void InitTheme()
    {
        int theme = PlayerPrefs.GetInt("theme", 0);
        SetTheme(theme);
    }   

    private void SetTheme(int theme)
    {
        PlayerPrefs.SetInt("theme", theme);
        ColourSwatchManager.SwitchSwatch(theme);

        switch(theme)
        {
            case 0:
                DarkTick.SetActive(false);
                LightTick.SetActive(true);

                break;
            case 1:
                LightTick.SetActive(false);
                DarkTick.SetActive(true);
                break;
        }
    }


}
