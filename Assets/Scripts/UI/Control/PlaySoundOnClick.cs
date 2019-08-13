using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Button))]
public class PlaySoundOnClick : MonoBehaviour
{

    private AudioSource mySource;

    private void Start() 
    {
        mySource = GetComponent<AudioSource>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        mySource.Play();
    }   
}
