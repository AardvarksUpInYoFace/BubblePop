using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoniUtility;
using UnityEngine.UI;

public class ScoreTextController : MonoBehaviour
{

    public Text myText;

    public void AnimateScoreRising(int Score)
    {
        myText.enabled = true;
        myText.text = Score.ToString();

        EasingActions easing = new EasingActions();

        StartCoroutine(easing.CoMoveY(0, 0.5f, transform.position.y, transform.position.y + 0.5f, transform, Easing.Function.Sinusoidal, Easing.Direction.Out));

        StartCoroutine(easing.CoFadeTextAlpha(0, 0.2f, 0f, 1f, myText, Easing.Function.Sinusoidal, Easing.Direction.Out));

        StartCoroutine(DelayedDestroy());
    }


    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }


}
