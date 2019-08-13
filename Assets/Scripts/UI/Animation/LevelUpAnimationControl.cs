using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using JoniUtility;

public class LevelUpAnimationControl : MonoBehaviour
{
 
    public GameObject LevelUpPanel;
    public Button CloseButton;
    public Text BannerText;
    public ParticleSystem PFX;

    public GunController GunController; 

    private EasingActions myEasing;

    private List<Image> LevelUpImages;
    private List<Text> LevelUpTexts;

    public float AnimationTime = 0.77f;
    public List<string> WinMessages;

    private void Start() 
    {
        myEasing = new EasingActions();

        LevelUpImages = LevelUpPanel.GetComponentsInChildren<Image>().ToList();
        //remove the background image so it isn't faded in.
        LevelUpImages.RemoveAt(0);
        LevelUpTexts = LevelUpPanel.GetComponentsInChildren<Text>().ToList();

        CloseButton.onClick.AddListener(OnClosePress);
    }

    public void OnLevelUp()
    {
        LevelUpPanel.SetActive(true);
        
        StartCoroutine(myEasing.CoScale(0, AnimationTime, 0.01f, 1f,LevelUpPanel.transform ,Easing.Function.Back, Easing.Direction.Out));

        //cycle through all the images to fade them in sine.
        foreach(Image image in LevelUpImages)
        {
            StartCoroutine(myEasing.CoFadeImageAlpha(0, AnimationTime, 0f, 1f, image, Easing.Function.Sinusoidal, Easing.Direction.Out));
        }
        foreach(Text text in LevelUpTexts)
        {
            StartCoroutine(myEasing.CoFadeTextAlpha(0, AnimationTime, 0f, 1f, text, Easing.Function.Sinusoidal, Easing.Direction.Out));
        }

        //pick a random level up message 
        //TODO: Make the messages shuffle rather than completely random.
        BannerText.text = WinMessages[Random.Range(0, WinMessages.Count)] + "!";
    }


    private void OnClosePress()
    {
        StartCoroutine(myEasing.CoScale(0, AnimationTime/3f, 1f, 0.01f,LevelUpPanel.transform ,Easing.Function.Back, Easing.Direction.In));

        foreach(Image image in LevelUpImages)
        {
            StartCoroutine(myEasing.CoFadeImageAlpha(0, AnimationTime/3f, 1f, 0f, image, Easing.Function.Sinusoidal, Easing.Direction.In));
        }
        foreach(Text text in LevelUpTexts)
        {
            StartCoroutine(myEasing.CoFadeTextAlpha(0, AnimationTime/3f, 1f, 0f, text, Easing.Function.Sinusoidal, Easing.Direction.In));
        }

        PFX.Play();

        StartCoroutine(DelayedSetInactive());
    }

    private IEnumerator DelayedSetInactive()
    {
        yield return new WaitForSeconds(AnimationTime/3f);
        LevelUpPanel.SetActive(false); 
        GunController.InMenu = false;
    }
}
