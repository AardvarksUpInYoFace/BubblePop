using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public LevelUpAnimationControl LevelUpAnimationControl;
    public Slider LevelSlider;
    public GridValueController GridValueController;
    public ColourSwatchManager ColourSwatchManager;
    public Text ScoreText, Level1Text, Level2Text, ScoreMultiplierText;
    public Image Level1Image, Level2Image, LevelSliderFill;
    public GunController GunController;

    private int CurrentLevel;
    public int Score, NextLevelScore;
    public int IntermediateScore;

    private int MagicScoreMultiplier = 5000;

    private int ScoreMultiplier = 1;

    public GameObject ScoreTextPrefab;

    private AudioSource mySource;

    private void Start() 
    {
        InitLevel();

        mySource = GetComponent<AudioSource>();
        mySource.Play();

        Application.targetFrameRate = 60;
    }

    private void InitLevel()
    {
        CurrentLevel = PlayerPrefs.GetInt("level", 1);
        Score = PlayerPrefs.GetInt("score", 0);
        IntermediateScore = PlayerPrefs.GetInt("xp", 0);

        SetMultiplier();

        NextLevelScore = Mathf.RoundToInt(MagicScoreMultiplier * (Mathf.Sqrt(CurrentLevel + 1) - Mathf.Sqrt(1)));

        Level1Text.text = CurrentLevel.ToString();
        Level2Text.text = (CurrentLevel + 1).ToString();

        // -1, as level is one indexed but the colours are zero indexed in an array
        LevelSliderFill.color = GetImageColor(CurrentLevel - 1);
        Level1Image.color = GetImageColor(CurrentLevel - 1);
        Level2Image.color = GetImageColor(CurrentLevel);

        OnScoreChange(0,1, new Vector3(-99f, -99f, -99f));
        GridValueController.SetupValues(CurrentLevel);
    }


    private Color GetImageColor(int level)
    {
        int modVal = (level % 8) + 2;

        float alpha = 1f;
        Color newColour = ColourSwatchManager.Swatches[ColourSwatchManager.CurrentSwatch].Colours[modVal];        
        return new Color(newColour.r, newColour.g, newColour.b, alpha);
    }

    public void OnScoreChange(int score, int multiplier, Vector3 position)
    {
        multiplier = Mathf.Min(multiplier, CurrentLevel);
        Score += score *multiplier;
        IntermediateScore += score *multiplier;
        PlayerPrefs.SetInt("score", Score);
        PlayerPrefs.SetInt("xp", IntermediateScore);

        ScoreText.text = ConvertNumber.ConvertNumberToLetteredString(Score);

        if(IntermediateScore >= NextLevelScore) LevelUp();

        LevelSlider.value = ((IntermediateScore+1f) / NextLevelScore);

        var obj = Instantiate(ScoreTextPrefab);
        obj.transform.position = new Vector3(position.x, position.y, -0.4f);
        obj.transform.localScale *= 0.33f;
        obj.GetComponent<ScoreTextController>().AnimateScoreRising(score*multiplier);
    }

    private void LevelUp()
    {
        GunController.InMenu = true;

        mySource.Play();

         LevelUpAnimationControl.OnLevelUp();
         CurrentLevel++;
         PlayerPrefs.SetInt("level", CurrentLevel);

        Level1Text.text = CurrentLevel.ToString();
        Level2Text.text = (CurrentLevel + 1).ToString();

        NextLevelScore = Mathf.RoundToInt(MagicScoreMultiplier * (Mathf.Sqrt(CurrentLevel + 1) - Mathf.Sqrt(1)));
        IntermediateScore = 0;

        SetMultiplier();

        GridValueController.OnLevelChange(CurrentLevel);
    }

    private void SetMultiplier()
    {
        ScoreMultiplier = 1 + Mathf.FloorToInt(CurrentLevel / 5);
        ScoreMultiplierText.text = "x" + ScoreMultiplier; 
    }
}
