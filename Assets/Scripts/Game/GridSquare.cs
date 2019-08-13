using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoniUtility;

public class GridSquare : MonoBehaviour
{

    public bool Occupied;
    public bool CanLand;
    public Text myText;
    public SpriteRenderer myCircle;
    public SpriteSwatchListener circleListener;
    public GridValueController ValueController;

    public Collider2D myCollider;

    public int Value = 0;

    public Vector2 LayerIndex;

    public void RandomOccupy()
    {
        OccupySetup();
        int index = Random.Range(0,ValueController.GridValues.Count);
        Value = ValueController.GridValues[index];
        SetVisuals();
    }

    private void SetVisuals()
    {
        myText.text = ConvertNumber.ConvertNumberToLetteredString(Value);

        circleListener.ColourIndex = ((int) Mathf.Log(Value, 2) % 8) + 2;
        circleListener.SetColour();
    }

    private void OccupySetup()
    {
        Occupied = true;
        myCircle.enabled = true;
        myText.enabled = true;
    }
    
    public void SetOccupied(int value)
    {
        Value = value;
        OccupySetup();
        SetVisuals();
    }

    public void UnOccupy()
    {
        Occupied = false;
        myCircle.enabled = false;
        myText.enabled = false;       
        //Value  = 0;
    }

    public void SetLanded(bool land)
    {
        CanLand = land;
        if(CanLand)
        {
            myCollider.enabled = true;
        }
        else myCollider.enabled = false;
    }


    public void PushedAnimation(Vector3 epicentre)
    {
        epicentre.Normalize();

        EasingActions easing = new EasingActions();
        StartCoroutine(easing.CoMoveY(0, 0.1f, transform.localPosition.y, transform.localPosition.y +epicentre.y * 0.1f, transform,Easing.Function.Sinusoidal,Easing.Direction.Out));
        StartCoroutine(easing.CoMoveX(0, 0.1f, transform.localPosition.x, transform.localPosition.x +epicentre.x * 0.1f, transform,Easing.Function.Sinusoidal,Easing.Direction.Out));
        StartCoroutine(PullBack(easing, epicentre));
    }

    private IEnumerator PullBack(EasingActions easing, Vector3 epicentre)
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(easing.CoMoveY(0, 0.1f, transform.localPosition.y, transform.localPosition.y - epicentre.y * 0.1f, transform,Easing.Function.Sinusoidal,Easing.Direction.In));
        StartCoroutine(easing.CoMoveX(0, 0.1f, transform.localPosition.x, transform.localPosition.x - epicentre.x * 0.1f, transform,Easing.Function.Sinusoidal,Easing.Direction.In));
    }

    public void CombineGridSquare(Vector3 position)
    {
        StopAllCoroutines();
        myCollider.enabled = false;
        CanLand = false;

        EasingActions easing = new EasingActions();
        StartCoroutine(easing.CoMoveY(0, 0.5f, transform.localPosition.y, position.y, transform,Easing.Function.Sinusoidal,Easing.Direction.Out));
        StartCoroutine(easing.CoMoveX(0, 0.5f, transform.localPosition.x, position.x, transform,Easing.Function.Sinusoidal,Easing.Direction.Out));
        StartCoroutine(easing.CoScale(0, 0.5f, 1f, 0.01f, transform,Easing.Function.Sinusoidal,Easing.Direction.Out));
        StartCoroutine(DelayedDestroy(0.5f));
    }

    public void FallGridSquare()
    {
        StopAllCoroutines();
        myCollider.enabled = false;
        CanLand = false;

        EasingActions easing = new EasingActions();
        StartCoroutine(easing.CoMoveY(0, 0.2f, transform.localPosition.y, transform.localPosition.y + 0.5f, transform,Easing.Function.Sinusoidal,Easing.Direction.Out));
        StartCoroutine(DelayedDownMovement(easing));
    }

    private IEnumerator DelayedDownMovement(EasingActions easing)
    {
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(easing.CoMoveY(0, 0.4f, transform.localPosition.y, -3f, transform,Easing.Function.Sinusoidal,Easing.Direction.In));
        StartCoroutine(DelayedDestroy(0.4f));
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
