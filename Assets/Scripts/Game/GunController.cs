using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoniUtility;

public class GunController : MonoBehaviour
{
    public Camera myCamera;
    public LineRenderer myLine;
    public GridManager GridManager;
    public float LerpSpeed = 0.3f;
    public float BulletSpeed = 0.1f;
    private float MagicAngle = 0.3f;
    private Vector3 CurrentPosition;
    public SpriteRenderer GhostCircle;
    public SpriteRenderer DummySprite;
    public GridSquare CurrentSquare, GhostSquare, NextSquare;
    private GridSquare SelectedGridSquare;
    public bool CanFire = true;
    public bool InMenu = false;
    private EasingActions myEasing;

    private float dummyScaleTime = 0.1f;

    private float CurrentZ;
    private Vector3 NextSquarePos;

    private AudioSource mySource;

    public void OnTouchDown()
    {
        CurrentPosition = myCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Start() 
    {
        myEasing = new EasingActions();
        mySource = GetComponent<AudioSource>();

        CurrentZ = CurrentSquare.transform.localPosition.z;
         NextSquarePos = NextSquare.transform.localPosition;

        var value = NextSquare.ValueController.GridValues[Random.Range(0,NextSquare.ValueController.GridValues.Count - 1)];
        NextSquare.SetOccupied(value);
        ReloadGun();
    }

    void Update()
    {
        if(!CanFire || InMenu) return;

        if(Input.GetMouseButtonDown(0)) OnTouchDown();

        if(Input.GetMouseButton(0)) RaycastToGrid();

        if(Input.GetMouseButtonUp(0) && myLine.positionCount > 1) FireCircle();
    }

    private void FireCircle()
    {
        CanFire = false;

        mySource.Play();

        Vector3[] linePos = new Vector3[myLine.positionCount];
        myLine.GetPositions(linePos);
        linePos[myLine.positionCount-1] = SelectedGridSquare.transform.position;

        GhostCircle.transform.position = transform.position;

        GhostSquare.SetOccupied(CurrentSquare.Value);
        CurrentSquare.UnOccupy();

        myLine.positionCount = 1;

        StartCoroutine(AnimateBulletPath(linePos, 0));
    }

    private void RaycastToGrid()
    {
        //smoothly move the line around for better feel.
        float xLerp = Mathf.Lerp(CurrentPosition.x, myCamera.ScreenToWorldPoint(Input.mousePosition).x, LerpSpeed);
        float yLerp = Mathf.Lerp(CurrentPosition.y, myCamera.ScreenToWorldPoint(Input.mousePosition).y, LerpSpeed);

        Vector3 lineEndPos = new Vector3(xLerp, yLerp, 0);
        CurrentPosition = lineEndPos;

        Vector3 direction = lineEndPos - transform.position;

        float angle = Mathf.Atan(direction.y/direction.x);


        //don't draw the line if to acute an angle
        if(direction.x > 0 && angle < MagicAngle ||
            direction.x < 0 && angle > -MagicAngle) 
        {
            ResetTargetting();
            return;
        }

        //recursive raycast check
        RaycastCheck(transform.position, direction, 1);
    }



    private void RaycastCheck(Vector2 startPos, Vector2 direction, int iterator)
    {
        //safety first :)
        if(iterator> 20) return;

        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 100f,1<<8);

        if (hit.collider != null)
        {
            float offset = direction.x < 0 ? 0.1f : - 0.1f;
            Vector2 fakedHitPoint = hit.point + new Vector2(offset,0);

            //set the last position to the bounce position
            myLine.positionCount = iterator+1;
            myLine.SetPosition(iterator,fakedHitPoint);  

            //start another raycast if wall
            if(hit.collider.CompareTag("Wall"))
            {  
                Vector2 reflected = new Vector2(-direction.x, direction.y);
                RaycastCheck(fakedHitPoint,reflected, iterator + 1);
            }

            //if we're done with the raycast
            //otherwise draw the ghost circle in the selected grid cell.
            else
            {
                //TODO: adjust the end position of the line renderer to be on a physical circle
    
                SelectedGridSquare = hit.collider.GetComponent<GridSquare>();

                if(!SelectedGridSquare.Occupied) DrawGhost(SelectedGridSquare.transform.position);
            }
        }
    }

    private void DrawGhost(Vector3 position)
    {
        //currently bugged - colour not changing correctly.
        GhostCircle.enabled = true;
        GhostCircle.transform.position = position;
        var colour = DummySprite.color;
        GhostCircle.color = new Color(colour.r, colour.g, colour.g, 0.5f);
    }

    private void ResetTargetting()
    {
        myLine.positionCount = 1;
        GhostCircle.enabled = false;
    }

    private IEnumerator AnimateBulletPath(Vector3[] nodes, int iterator)
    {
        while(iterator + 1 < nodes.Length)
        {
            var difference = nodes[iterator + 1] - nodes[iterator];
            difference = difference.normalized;

            GhostCircle.transform.position +=  difference * BulletSpeed;

            if(GhostCircle.transform.position.y > nodes[iterator + 1].y) 
            {
                GhostCircle.transform.position = nodes[iterator + 1];
                iterator++;
            }

            yield return 0;
        }

        SelectedGridSquare.SetOccupied(CurrentSquare.Value);


        GridManager.OnGridChanged(SelectedGridSquare);

        ReloadGun();
    }


    private void ReloadGun()
    {
        CanFire = true;
        GhostSquare.UnOccupy();

        CurrentSquare.SetOccupied(NextSquare.Value);

        //gun values are minus the highest board value.
        var value = NextSquare.ValueController.GridValues[Random.Range(0,NextSquare.ValueController.GridValues.Count - 1)];
        NextSquare.SetOccupied(value);

        AnimateDummyReload();   
    }

    private void AnimateDummyReload()
    {
        CurrentSquare.transform.localPosition = new Vector3(NextSquarePos.x, NextSquarePos.y, CurrentZ);
        CurrentSquare.transform.localScale =  NextSquare.transform.localScale;

        StartCoroutine(myEasing.CoScale(0, dummyScaleTime, CurrentSquare.transform.localScale.x, 1, CurrentSquare.transform,Easing.Function.Sinusoidal,Easing.Direction.Out));
        StartCoroutine(CoDelayMoveXDummy());
    }

    private IEnumerator CoDelayMoveXDummy()
    {
        yield return new WaitForSeconds(dummyScaleTime);

        StartCoroutine(myEasing.CoMoveX(0, dummyScaleTime,CurrentSquare.transform.localPosition.x, 0,CurrentSquare.transform, Easing.Function.Sinusoidal,Easing.Direction.Out));
    }
}
