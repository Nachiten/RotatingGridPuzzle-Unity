using System;
using UnityEngine;

public abstract class GridObjective : MonoBehaviour
{
    public static event Action<GridPosition> OnAnyGridObjectiveCompleted;
    public static event Action<GridPosition> OnAnyGridObjectiveUncompleted;
    public static event Action<GridPosition> OnAnyGridObjectiveSpawned;
    
    private GridPosition gridPosition;
    private bool isCompleted;
    
    private void Start()
    {
        GetComponent<GridObjectiveMaterial>().SetMaterial(GetObjectiveType());
        
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.OnAnyGridElementMovedGridPosition += OnAnyGridElementMovedGridPosition;
        
        OnAnyGridObjectiveSpawned?.Invoke(gridPosition);
    }
    
    private void OnAnyGridElementMovedGridPosition(GridElement gridElement, GridPosition fromGridPos, GridPosition toGridPos)
    {
        switch (isCompleted)
        {
            // If its not completed and it moved to this grid pos
            case false when toGridPos == gridPosition:
            {
                // If its the correct objective type
                if (!IsCorrectGridElementType(gridElement)) 
                    return;
                
                // Complete it
                isCompleted = true;
                OnAnyGridObjectiveCompleted?.Invoke(gridPosition);
                break;
            }
            // If it was completed and it moved out of this grid pos
            case true when fromGridPos == gridPosition:
            {
                // If its the correct objective type
                if (!IsCorrectGridElementType(gridElement))
                    return;

                // Uncomplete it
                isCompleted = false;
                OnAnyGridObjectiveUncompleted?.Invoke(gridPosition);

                break;
            }
        }
    }
    
    protected abstract bool IsCorrectGridElementType(GridElement gridElement);
    
    protected abstract ObjectiveType GetObjectiveType();
}
