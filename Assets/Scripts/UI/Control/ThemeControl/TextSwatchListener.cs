using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextSwatchListener : MonoBehaviour
{
    public int ColourIndex;
    public ColourSwatchManager myManager;
    private Text myText;

    private void OnEnable() 
    {
        myText = GetComponent<Text>();
        myManager.OnSwatchChange += ChangeColour;

        ChangeColour(myManager.CurrentSwatch);
    }


    private void ChangeColour(int swatch)
    {
        float alpha = myText.color.a;
        Color newColour = myManager.Swatches[swatch].Colours[ColourIndex];      
        myText.color = new Color(newColour.r, newColour.g, newColour.b, alpha);
    }


    private void OnDisable() 
    {
        myManager.OnSwatchChange -= ChangeColour;
    }

}
