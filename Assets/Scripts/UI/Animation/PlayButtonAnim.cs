using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoniUtility;


public class PlayButtonAnim : MonoBehaviour
{

    private EasingActions myEasing;

    public float AnimationTime = 0.1f;
    public float AnimationSizeMax = 1.1f;

    private void OnEnable()
    {
        myEasing = new EasingActions();

        StartCoroutine(myEasing.CoScaleLoop(0, AnimationTime, transform.localScale.x, AnimationSizeMax,transform,Easing.Function.Sinusoidal, Easing.Direction.Out));
    }

    private void OnDisable() 
    {
        StopAllCoroutines();
    }
}
