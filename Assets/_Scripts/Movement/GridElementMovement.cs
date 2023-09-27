using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridElementMovement : MonoBehaviour
{
    public static GridElementMovement Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Move a GridElement from the given grid position in the given direction
    /// </summary>
    /// <param name="fromGridPos"> The origin grid position </param>
    /// <param name="direction"> The direction to move the GridElement in </param>
    private List<GridElement> MoveGridElement(GridPosition fromGridPos, GridPosition direction)
    {
        List<GridElement> gridElementsMoved = new();

        // Get the grid element at the origin grid pos
        GridElement gridElementAtGridPos = LevelGrid.Instance.GetGridElementAtGridPos(fromGridPos);
        
        gridElementsMoved.Add(gridElementAtGridPos);
        
        // Get the grid positions occupied by the grid element at target pos
        List<GridPosition> gridPositionsToMoveTo = gridElementAtGridPos.GetGridPositionsForDirection(direction);
        
        //PrintGridPositionsList(gridPositionsToMoveTo, "Grid positions to move to");
        
        // Cycle through all grid positions at target pos
        gridPositionsToMoveTo.ForEach(_fromGridPos =>
        {
            // Calculate toGridPos for this _fromGridPos
            GridPosition _toGridPos = _fromGridPos + direction;
    
            // Can NOT move to grid position, target grid pos invalid
            if (!LevelGrid.Instance.ValidGridPosToMove(_toGridPos))
            {
                // This should not happen, because we scan if it is possible before executing
                Debug.LogError("Should not happen: Invalid grid position to move to");
                return;
            }

            // CAN move to grid position, grid position empty
            if (!LevelGrid.Instance.GridPosHasAnyGridElement(_toGridPos))
            {
                // Get the GridElement at the origin grid pos
                GridElement gridElementAtFromGridPos = LevelGrid.Instance.GetGridElementAtGridPos(_fromGridPos);

                // The grid element that started this call is moved after the ones after it finish moving
                if (gridElementAtFromGridPos == gridElementAtGridPos)
                    return;

                // Debug log from which to which grid pos I moved
                // Debug.Log($"Moved {gridElementAtFromGridPos.name} " +
                //           $"from [{_fromGridPos.ToString()}] " +
                //           $"to [{_toGridPos.ToString()}]");

                // Move the grid element at the origin grid pos to the target grid pos
                gridElementAtFromGridPos.MoveGridElementInDirection(direction);
                return;
            }
           
            // If base cases are not met, continue the problem forward recursively until reaching a base case (can move, or reach a barrier)
            gridElementsMoved.AddRange(MoveGridElement(_toGridPos, direction));
        });
    
        // The grid element that started this call is moved now
        gridElementAtGridPos.MoveGridElementInDirection(direction);
        return gridElementsMoved;
    }

    public void MoveGridElements(List<GridPosition> gridPositions, GridPosition direction)
    {
        List<GridElement> gridElementsMoved = new();
        
        gridPositions.ForEach(gridPosition =>
        {
            gridElementsMoved.AddRange(MoveGridElement(gridPosition, direction));
        });
        
        HistoryManager.Instance.AddHistory(gridElementsMoved, direction);
    }

    /// <summary>
    /// Check if the GridElements at the given grid positions can be moved in the given direction
    /// </summary>
    /// <param name="gridPositions"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool CanMoveGridElements(List<GridPosition> gridPositions, GridPosition direction)
    {
        return gridPositions.All(gridPosition =>
        {
            GridPosition toGridPos = gridPosition + direction;

            if (!LevelGrid.Instance.ValidGridPosToMove(toGridPos))
            {
                //Debug.Log("Can't move GridElements");
                //PrintGridPositionsList(gridPositions, "Grid positions to move");
                return false;
            }

            return !LevelGrid.Instance.GridPosHasAnyGridElement(toGridPos) || 
                   CanMoveGridElements(LevelGrid.Instance.GetGridElementAtGridPos(gridPosition + direction)
                       .GetGridPositionsForDirection(direction), direction);
        });
    }

    // private void PrintGridPositionsList(List<GridPosition> gridPositions, string listName = "Grid Positions")
    // {
    //     Debug.Log($"----- {listName} ------");
    //     
    //     gridPositions.ForEach(gridPosition =>
    //     {
    //         Debug.Log(gridPosition + " | ");
    //     });
    //     
    //     Debug.Log("---------------------------");
    // }
    
    // private void PrintGridElementsList(List<GridElement> gridElements, string listName = "Grid Elements")
    // {
    //     Debug.Log($"----- {listName} ------");
    //     
    //     gridElements.ForEach(gridElement =>
    //     {
    //         Debug.Log(gridElement + " | ");
    //     });
    //     
    //     Debug.Log("---------------------------");
    // }
}
