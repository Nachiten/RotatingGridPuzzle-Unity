using UnityEngine;

public static class UnitMovement
{
    public static bool TryMoveUnit(GridPosition fromGridPos, GridPosition toGridPos)
    {
        // The unit can move if:
        // 1. The destination grid pos is valid AND ->
        // 2. The destination grid pos is empty OR the next units recursivly move
        if (ValidGridPosToMove(toGridPos) && 
            (!LevelGrid.Instance.GridPosHasAnyUnit(toGridPos) || 
             TryMoveUnit(toGridPos, CalculateNextTryingGridPos(fromGridPos, toGridPos))))
        {
            // Get the unit at the origin grid pos
            Unit unitAtFromGridPos = LevelGrid.Instance.GetUnitAtGridPos(fromGridPos);
            
            // Debug log from which to which grid pos I moved
            Debug.Log("Moved from [" + fromGridPos.ToOneLineString() + "] to [" + toGridPos.ToOneLineString() + "]");
                        
            // First unit can move only if all the next units could move
            unitAtFromGridPos.MoveUnitToGridPosition(toGridPos);
            return true;
        }
        
        return false;
    }

    private static bool ValidGridPosToMove(GridPosition gridPos)
    {
        return LevelGrid.Instance.GridPosIsValid(gridPos) && LevelGrid.Instance.GridPosIsWalkable(gridPos);
    }

    private static GridPosition CalculateNextTryingGridPos(GridPosition fromGridPos, GridPosition toGridPos)
    {
        // Calculate linear signed distance for each axis
        int distanceX = toGridPos.x - fromGridPos.x;
        int distanceZ = toGridPos.z - fromGridPos.z;

        // Get the grid pos behind the unit
        return new GridPosition(toGridPos.x + distanceX, toGridPos.z + distanceZ, toGridPos.floor);
    }
}
