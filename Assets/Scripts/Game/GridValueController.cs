using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu]
public class GridValueController : ScriptableObject
{

    //every n levels add a new highest number
    //every m levels remove the lowest number

    public float HigherNumberAdditionalInterval = 4f;
    public float LowerNumberRemovalInterval = 8f;

    public List<int> GridValues = new List<int> {2,4,8,16};

    //gun values = grid values - the highest number.

    public void SetupValues(int CurrentLevel)
    {
        GridValues = new List<int> {2,4,8,16};

        int GridChanges =Mathf.FloorToInt(CurrentLevel / HigherNumberAdditionalInterval);
        int LevelChanges = Mathf.FloorToInt(CurrentLevel / LowerNumberRemovalInterval);

        for(int i =0; i < GridChanges; i++)
        {
            AddGridValue();
        }

        for(int j = 0; j < LevelChanges; j++)
        {
            RemoveGridValue();
        }      
    }

    public void OnLevelChange(int newLevel)
    {
        if(newLevel % HigherNumberAdditionalInterval == 0)
        {
            AddGridValue();
        }

        if(newLevel % LowerNumberRemovalInterval == 0)
        {
            RemoveGridValue();
        }
    }

    private void RemoveGridValue()
    {
        GridValues.RemoveAt(0);
    }

    private void AddGridValue()
    {
        GridValues.Add(GridValues[GridValues.Count -1]*2);
    }

/*
    public int ConvertValueToIndex(int Value)
    {
        if (GridValues.Exists(x => x == Value))
        {
            int index = GridValues.FindIndex(x => x == Value);
            return index;
        }
        else
        {
            return (int) Mathf.Log(Value,2) - (GridValues[0] - 1);
        }
    }
*/

}
