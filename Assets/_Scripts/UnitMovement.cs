using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public static UnitMovement Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }
    
    private void InitializeSingleton()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of UnitMovement found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool TryMoveUnit(GridPosition fromGridPos, GridPosition toGridPos)
    {
        // Get unit at target grid pos
        Unit unitAtFromGridPos = LevelGrid.Instance.GetUnitAtGridPos(fromGridPos);

        if (!LevelGrid.Instance.GridPosIsValid(toGridPos))
        {
            // Grid pos is not valid, outside of grid
            return false;
        }

        if (!LevelGrid.Instance.GridPosHasAnyUnit(toGridPos))
        {
            // Unit can move to grid pos
            unitAtFromGridPos.MoveUnitToGridPosition(toGridPos);
            return true;
        }

        int distanceX = toGridPos.x - fromGridPos.x;
        int distanceZ = toGridPos.z - fromGridPos.z;

        // Check if grid pos + distance is valid and has any unit
        GridPosition newTryingGridPos = new GridPosition(toGridPos.x + distanceX, toGridPos.z + distanceZ, toGridPos.floor);
        
        Unit unitAtToGridPos = LevelGrid.Instance.GetUnitAtGridPos(toGridPos);

        if (!TryMoveUnit(toGridPos, newTryingGridPos)) 
            return false;
        
        // Unit can move to grid pos
        unitAtToGridPos.MoveUnitToGridPosition(newTryingGridPos);
        return true;
    }
}
