using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public Button StartButton, SettingsButton, PausePanel;

    public GameObject SettingsCanvas;
    private GameObject MenuCanvas;

    public GunController GunController;


    private void Start() 
    {
        MenuCanvas = gameObject;

        StartButton.onClick.AddListener(OnStartPress);
        SettingsButton.onClick.AddListener(OnSettingsPress);
        PausePanel.onClick.AddListener(OnPausePress);

        GunController.InMenu = true;
    }

    private void OnStartPress()
    {
        StartCoroutine(CoDelayedCanFire());
    }

    private IEnumerator CoDelayedCanFire()
    {
        yield return new WaitForSeconds(0.1f);
        MenuCanvas.SetActive(false);
        GunController.InMenu = false;
    }

    private void OnSettingsPress()
    {
        SettingsCanvas.SetActive(true);
    }

    private void OnPausePress()
    {
        MenuCanvas.SetActive(true);
        GunController.InMenu = true;
    }
}
