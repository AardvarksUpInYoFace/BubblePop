using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{
    public Button BackButton, ThemeButton, ThemeBackButon;
    private GameObject SettingsCanvas;
    public GameObject ThemePanel;
    public Slider SoundSlider;

    public AudioMixer SFXMixer;

    private void Start() 
    {   
        SettingsCanvas = gameObject;
        BackButton.onClick.AddListener(OnBackButtonPress);

        ThemeButton.onClick.AddListener(OnThemeButtonPress);
        ThemeBackButon.onClick.AddListener(OnThemeBackPress);

        SoundSlider.onValueChanged.AddListener(SFXChange);

        InitSFX();
    }

    private void OnBackButtonPress()
    {
        SettingsCanvas.SetActive(false);
    }

    private void OnThemeButtonPress()
    {
        ThemePanel.SetActive(true);
    }

    private void OnThemeBackPress()
    {
        ThemePanel.SetActive(false);
    }

    private void InitSFX()
    {
        float vol = PlayerPrefs.GetFloat("SFXVol", 0);

        if(vol >= 0f) 
        {
            SFXChange(1f);
            SoundSlider.value = 1f;
        }
        else SFXChange(0f);
    }


    private void SFXChange(float value)
    {
        int intVal = Mathf.RoundToInt(value);

        switch(intVal)
        {
            case 0:
                SFXMixer.SetFloat("SFXVolume", -80f);
                break;
            case 1:
                SFXMixer.SetFloat("SFXVolume", 0f);
                break; 
        }
        
        float volume;
        SFXMixer.GetFloat("SFXVolume", out volume);

        PlayerPrefs.SetFloat("SFXVol",volume);
    }



}
