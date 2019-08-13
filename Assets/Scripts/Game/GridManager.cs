using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JoniUtility;


[System.Serializable]
public class GridRow
{
    public int Layer;
    public GridSquare[] Row;

    public GridRow()
    {
        Row = new GridSquare[6];
    }
}

public class GridManager : MonoBehaviour
{

    //draw a grid of squares offset.

    public GameObject GridSquarePrefab;
    public GridValueController GridValueController;
    private int RowLength = 6;
    private static float MagicSpacing = 0.73143f;
    private Vector3 startPos = new Vector3(-2.05f, 4.95f, 0f);
    private Vector3 oddOffset = new Vector3(MagicSpacing /2f, 0f, 0f);
    public List<GridRow> GridCells = new List<GridRow>();
    public List<GridSquare> HitCells;
    public GameManager GameManager;
    private EasingActions myEasing;
    private List<GridSquare> CheckedCells;
    public PFXPooler PFXPooler;
    public SFXPooler SFXPooler;

    public GridSquare FallSquarePrefab;
    private void Start() 
    {
        myEasing = new EasingActions();
        InitGrid();
    }


    private void InitGrid()
    {
        GridCells.Add(InitRow(0,true));
        GridCells.Add(InitRow(1,true));
        GridCells.Add(InitRow(2,true));
        GridCells.Add(InitRow(3,true));
        GridCells.Add(InitRow(4,false));
    }

    private void RemoveRow(GridRow row)
    {
        GridCells.Remove(row);

        foreach(GridSquare square in row.Row)
        {
            Destroy(square.gameObject);
        }
    }


    private GridRow InitRow(int layer, bool occupy, bool above = false)
    {
        GridRow Row = new GridRow();
        Row.Layer = layer;

        float time = 0f;

        for(int i = 0;i < RowLength;i++)
        {
            var obj = Instantiate(GridSquarePrefab, transform);
            var oddset = (Mathf.Abs(layer) % 2 == 1)? oddOffset : Vector3.zero;
            //if(above) oddset = (GetLowestLayer -1 % 2 == 1)? oddOffset : Vector3.zero;
            obj.transform.localPosition = startPos + new Vector3(i* MagicSpacing,-layer*MagicSpacing*0.9f, 0) + oddset;
            var gridSquare = obj.GetComponent<GridSquare>();
            if(occupy)
            {
                gridSquare.RandomOccupy();
                StartCoroutine(myEasing.CoScale(0, 0.5f+ time, 0.01f, 1f, gridSquare.transform, Easing.Function.Sinusoidal, Easing.Direction.Out));
            } 
            gridSquare.SetLanded(true);
            Row.Row[i] = gridSquare;

            gridSquare.LayerIndex = new Vector2(Row.Layer, i);

            time += 0.05f;
        }
        return Row;     
    }

    private int GetHighestLayer()
    {
        int layer = -999;
        foreach(GridRow row in GridCells)
        {
            if(layer < row.Layer)
            {
                layer = row.Layer;
            }
        }
        return layer;
    }

    private int GetLowestLayer()
    {
        int layer = 999;
        foreach(GridRow row in GridCells)
        {
            if(layer > row.Layer)
            {
                layer = row.Layer;
            }
        }
        return layer;
    }

    public void OnGridChanged(GridSquare cell)
    {
        GridChanging(cell, 0);

        CheckForFalling();

        CheckToAddRowsBelow(cell);

        CheckLandable();

        CheckToAddRowsAtTop(); 
    }

    private void GridChanging(GridSquare cell, int iterator)
    {
        HitCells = new List<GridSquare>();
        HitCells.Add(cell);

        List<GridSquare> SurrondingCells = CheckSurrondingCells(cell);
        foreach(GridSquare square in SurrondingCells) square.PushedAnimation(cell.transform.localPosition);

        CheckCombiningCells(cell, iterator);
    }

    private void CheckForFalling()
    {
        CheckedCells = new List<GridSquare>();

        List<GridSquare> AllCells = GetComponentsInChildren<GridSquare>().ToList();
        AllCells.Reverse();

        foreach(GridSquare cell in AllCells)
        {
            if(!cell.Occupied) continue;

            List<GridSquare> ConnectedCells = new List<GridSquare>();
            ConnectedCells.Add(cell);
            CheckCellCeiling(cell, ConnectedCells);
        }

        //recursively check through all cells starting at the bottom

        //if there are no cells attached at the sides or the top of the cell - then that cell is falling.
        //any cells connected to that cell are falling as well

        //otherwise if you are connected - check the cells you are connected to.
        //if you are cell on the lowest layer then break out.
    }

    private void CheckCellCeiling(GridSquare cell, List<GridSquare> ConnectedCells)
    {
        if(CheckedCells.Contains(cell)) return;

        CheckedCells.Add(cell);
        if(cell.LayerIndex.x == GetLowestLayer()) return;

        List<GridSquare> SurrondingCells = CheckCellsAboveAndSides(cell);

        if(SurrondingCells.Count < 1)
        {
            //this cell is falling and all cells attached to it are falling!
            foreach(GridSquare square in ConnectedCells)
            {
                square.UnOccupy();
                GameManager.OnScoreChange(square.Value, 1, square.transform.position);

                var obj = Instantiate(FallSquarePrefab);
                obj.transform.position = square.transform.position; 
                var script =  obj.GetComponent<GridSquare>();
                script.SetOccupied(square.Value);
                script.FallGridSquare();
            } 

            return;
        }

        foreach(GridSquare square in SurrondingCells) 
        {
            ConnectedCells.Add(square);
            CheckCellCeiling(square, ConnectedCells);
        }
    }

    private void CheckToAddRowsBelow(GridSquare cell)
    {
        //if cell is occupied and on the highest row.
        //add a new row - then set all of them unlandable, other than the one's adjacent to current cell.
        int highest = GetHighestLayer();

        if(cell.Occupied && cell.LayerIndex.x == highest)
        {
            var newRow = InitRow(highest+1,false);
            GridCells.Add(newRow); 

            var SurrondingCells = CheckSurrondingCells(cell, true); 

            foreach(GridSquare square in newRow.Row)
            {
                if(!SurrondingCells.Contains(square))
                {
                    square.SetLanded(false);
                }
            }

            if(GridCells.Count > 8)  
            {
                StartCoroutine(myEasing.CoMoveY(0,0.5f,transform.localPosition.y, transform.localPosition.y + MagicSpacing*0.9f, transform, Easing.Function.Sinusoidal, Easing.Direction.Out));
                RemoveRow(GridCells.Find(x => x.Layer == GetLowestLayer()));

            }
        }

    }

    private void CheckLandable()
    {
        //going to check through every single cell to make sure it is landable
        //not necessarily efficient as O(n) but simpler to calculate and n is small always < 100.

        List<GridRow> rowsToRemove = new List<GridRow>();

        foreach(GridRow Row in GridCells)
        {
            int unlandables = 0;

            int lowest = GetLowestLayer();

            foreach(GridSquare square in Row.Row)
            {
                //if there is nothing above me and I become unlandable
                var SurrondingCells = CheckCellsAbove(square); 
                if(SurrondingCells.Count < 1 && Row.Layer != lowest) 
                {
                    square.SetLanded(false);
                    unlandables++;
                }
                else if(!square.Occupied) square.SetLanded(true);
            }
            //if everything on my row is unlandable - delete that row.
            if(unlandables == 6)
            {
                rowsToRemove.Add(Row);
            }
        }

        foreach(GridRow row in rowsToRemove)
        {
            RemoveRow(row);
        }
    }

    private void CheckToAddRowsAtTop()
    {
        if(GridCells.Count < 5)
        {
        StartCoroutine(myEasing.CoMoveY(0,0.5f,transform.localPosition.y, transform.localPosition.y - MagicSpacing*0.9f, transform, Easing.Function.Sinusoidal, Easing.Direction.Out));
        GridCells.Add(InitRow(GetLowestLayer() -1,true, true));
        }
    }

    private void CheckCombiningCells(GridSquare cell, int iterator)
    {     

        //once we have all the surronding same value cells, figue out what the new value is, and see if there is a cell with that value nearby to combine to
        if(HitCells.Count < 2) return;

        cell.UnOccupy();

        GridSquare CombineCell = HitCells[1];
        bool hasMatched = false;

        int newValue = cell.Value * (int) Mathf.Pow(2,HitCells.Count - 1);
        
        //foreach cell in the hit cells, check the surronding cels of each one and see if there's the new value adjacent to them
        foreach(GridSquare square in HitCells)
        {
            var SurrondingCells = CheckSurrondingCells(square);
            foreach(GridSquare adjacent in SurrondingCells)
            {
                if(adjacent.Value == newValue && !hasMatched)
                {
                    //found a match and will combine to that one after this is over.
                    CombineCell = square;
                    hasMatched = true;
                    break;
                }

                square.UnOccupy();
            }        
        }

        if(!hasMatched) CombineCell = HitCells[HitCells.Count - 1];
        
        //at that cell, occupy with the new value.
        CombineCell.SetOccupied(newValue);

        //animate the combination
        foreach(GridSquare square in HitCells)
        {
            if(square == CombineCell) continue;

            var obj = Instantiate(FallSquarePrefab);
            obj.transform.position = square.transform.position; 
            var script =  obj.GetComponent<GridSquare>();
            script.SetOccupied(square.Value);
            script.CombineGridSquare(CombineCell.transform.position);
        }

        GameManager.OnScoreChange(newValue, HitCells.Count, CombineCell.transform.position);
        PFXPooler.PlayPFX(CombineCell.transform.position);
        SFXPooler.PlaySFX(newValue);

        //TODO: if that cell now has a value > 2048, explode

         //once done check call GridChanging at the new cell.
        GridChanging(CombineCell, iterator + 1);
    }

    private List<GridSquare> CheckSurrondingCells(GridSquare cell, bool CheckUnOccupied = false)
    {
        Vector2 CellIndex = cell.LayerIndex;

        List<GridSquare> SurrondingCells = new List<GridSquare>();

        //search different cells depending if on an odd or even row.
        //even - check y-1 and y on both above and below rows.
        int left = 0, right = 0;

        if(CellIndex.x % 2 == 0)
        {
            left = -1;
            right = 0; 
        }
        //odd - check y and y +1 on both above and below rows.
        else
        {
            left = 0;
            right = 1;
        }

        //on your row
        GridRow row = GridCells.Find(x => x.Layer == CellIndex.x);
        SurrondingCells.AddRange(FindAdjacentCellsInRow(CellIndex, row, -1, 1,CheckUnOccupied));

        //row above
        row = GridCells.Find(x => x.Layer == CellIndex.x - 1);
        SurrondingCells.AddRange(FindAdjacentCellsInRow(CellIndex, row, left, right,CheckUnOccupied));

        //row below
        row = GridCells.Find(x => x.Layer == CellIndex.x + 1);
        SurrondingCells.AddRange(FindAdjacentCellsInRow(CellIndex, row, left, right,CheckUnOccupied)); 

        //find all surronding cells with the same value
        //recursively do this check to find all connecting matching values.
        foreach(GridSquare square in SurrondingCells)
        {
            if(!HitCells.Contains(square) && square.Value == cell.Value) 
            {
                HitCells.Add(square);
                CheckSurrondingCells(square);
            }
        }

        return SurrondingCells;
    }

    private List<GridSquare> CheckCellsAboveAndSides(GridSquare cell, bool CheckUnOccupied = false)
    {
        Vector2 CellIndex = cell.LayerIndex;

        List<GridSquare> SurrondingCells = new List<GridSquare>();

        //search different cells depending if on an odd or even row.
        //even - check y-1 and y on both above and below rows.
        int left = 0, right = 0;

        if(CellIndex.x % 2 == 0)
        {
            left = -1;
            right = 0; 
        }
        //odd - check y and y +1 on both above and below rows.
        else
        {
            left = 0;
            right = 1;
        }

        //on your row
        GridRow row = GridCells.Find(x => x.Layer == CellIndex.x);
        SurrondingCells.AddRange(FindAdjacentCellsInRow(CellIndex, row, -1, 1,CheckUnOccupied));

        //row above
        row = GridCells.Find(x => x.Layer == CellIndex.x - 1);
        SurrondingCells.AddRange(FindAdjacentCellsInRow(CellIndex, row, left, right,CheckUnOccupied));

        return SurrondingCells;
    }

    private List<GridSquare> CheckCellsAbove(GridSquare cell, bool CheckUnOccupied = false)
    {
        Vector2 CellIndex = cell.LayerIndex;

        List<GridSquare> SurrondingCells = new List<GridSquare>();

        //search different cells depending if on an odd or even row.
        //even - check y-1 and y on both above and below rows.
        int left = 0, right = 0;

        if(CellIndex.x % 2 == 0)
        {
            left = -1;
            right = 0; 
        }
        //odd - check y and y +1 on both above and below rows.
        else
        {
            left = 0;
            right = 1;
        }

        //row above
        var row = GridCells.Find(x => x.Layer == CellIndex.x - 1);
        SurrondingCells.AddRange(FindAdjacentCellsInRow(CellIndex, row, left, right,CheckUnOccupied));

        return SurrondingCells;
    }

    private List<GridSquare> FindAdjacentCellsInRow(Vector2 CellIndex, GridRow row, int left, int right, bool FindUnOccupied)
    {
            List<GridSquare> cells = new List<GridSquare>();

            if(row != null)
            {
                if(CellIndex.y + left > -1 && CellIndex.y + left < 6)
                {
                    GridSquare adjacentCell = row.Row[(int) CellIndex.y + left];
                    if(adjacentCell.Occupied || FindUnOccupied) cells.Add(adjacentCell);
                    
                }

                if(CellIndex.y + right > -1  && CellIndex.y + right < 6)
                {

                    GridSquare adjacentCell = row.Row[(int) CellIndex.y + right];
                    if(adjacentCell.Occupied || FindUnOccupied) cells.Add(adjacentCell);
                }
            }

            return cells;
    }
}
