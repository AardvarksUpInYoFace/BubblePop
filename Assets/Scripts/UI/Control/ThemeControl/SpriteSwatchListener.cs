using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwatchListener : MonoBehaviour
{
    public int ColourIndex;
    public ColourSwatchManager myManager;
    private SpriteRenderer mySprite;

    private void OnEnable() 
    {
        mySprite = GetComponent<SpriteRenderer>();
        myManager.OnSwatchChange += ChangeColour;

        ChangeColour(myManager.CurrentSwatch);
    }


    private void ChangeColour(int swatch)
    {
        float alpha = mySprite.color.a;
        Color newColour = myManager.Swatches[swatch].Colours[ColourIndex];      
        mySprite.color = new Color(newColour.r, newColour.g, newColour.b, alpha);
    }

    public void SetColour()
    {
        ChangeColour(myManager.CurrentSwatch);
    }


    private void OnDisable() 
    {
        myManager.OnSwatchChange -= ChangeColour;
    }
}
