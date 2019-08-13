using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ColourSwatch
{
    public Color[] Colours = new Color[10];
}


[System.Serializable]
[CreateAssetMenu]
public class ColourSwatchManager : ScriptableObject
{
    public List<ColourSwatch> Swatches = new List<ColourSwatch>();

    public int CurrentSwatch{get; private set;}

    public System.Action<int> OnSwatchChange;

    private void Awake() 
    {
        //try to avoid unity SO interactions vs editor (awake and enable do not run reliably in the same order for different objects).
        CurrentSwatch = PlayerPrefs.GetInt("theme", 0);
    }

    public void SwitchSwatch(int swatch)
    {
        CurrentSwatch = swatch;

        OnSwatchChange?.Invoke(CurrentSwatch);
    }
}
