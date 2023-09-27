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
    private void MoveGridElement(GridPosition fromGridPos, GridPosition direction)
    {
        GridElement gridElementAtGridPos = LevelGrid.Instance.GetGridElementAtGridPos(fromGridPos);
        List<GridPosition> gridPositionsToMoveTo = gridElementAtGridPos.GetGridPositionsForDirection(direction);
        
        //PrintGridPositionsList(gridPositionsToMoveTo, "Grid positions to move to");
        
        // Cycle through all grid positions in the given direction
        gridPositionsToMoveTo.ForEach(_fromGridPos =>
        {
            GridPosition _toGridPos = _fromGridPos + direction;
    
            // Can NOT move to grid position, grid position invalid
            if (!LevelGrid.Instance.ValidGridPosToMove(_toGridPos))
            {
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

                gridElementAtFromGridPos.MoveGridElementInDirection(direction);
                return;
            }
           
            // If base cases are not met, move the GridElement at toGridPos in the direction
            MoveGridElement(_toGridPos, direction);
        });
    
        // The grid element that started this call is moved now
        gridElementAtGridPos.MoveGridElementInDirection(direction);
    }

    public void MoveGridElements(List<GridPosition> gridPositions, GridPosition direction)
    {
        gridPositions.ForEach(gridPosition =>
        {
            MoveGridElement(gridPosition, direction);
        });
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

    // public void PrintGridPositionsList(List<GridPosition> gridPositions, string listName = "Grid Positions")
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
}
