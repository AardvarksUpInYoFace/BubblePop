using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageSwatchListener : MonoBehaviour
{
    public int ColourIndex;
    public ColourSwatchManager myManager;
    private Image myImage;

    private void OnEnable() 
    {
        myImage = GetComponent<Image>();
        myManager.OnSwatchChange += ChangeColour;

        ChangeColour(myManager.CurrentSwatch);
    }

    private void ChangeColour(int swatch)
    {
        float alpha = myImage.color.a;
        Color newColour = myManager.Swatches[swatch].Colours[ColourIndex];      
        myImage.color = new Color(newColour.r, newColour.g, newColour.b, alpha);
    }


    private void OnDisable() 
    {
        myManager.OnSwatchChange -= ChangeColour;
    }

}
