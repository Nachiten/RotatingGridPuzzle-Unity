using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridElement : MonoBehaviour
{
    private const float moveSpeed = 20f;
    protected GridPosition centerGridPosition;
    protected List<GridPosition> gridPositions;
    protected Vector3 targetPosition;
    protected bool isMoving;

    private void Awake()
    {
        gridPositions = new List<GridPosition>();
    }

    protected virtual void Start()
    {
        centerGridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.AddGridElementAtGridPos(centerGridPosition, this);
        gridPositions.Add(centerGridPosition);
    }

    protected virtual void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        if (!isMoving)
            return; 
        
        // Calculate new position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        if (transform.position == targetPosition)
            isMoving = false;
    }

    public void MoveGridElementInDirection(GridPosition direction)
    {
        MoveGridPositionsInDirection(direction);
    }

    private void MoveGridPositionsInDirection(GridPosition direction)
    {
        // Order the gridPositions depending on direction
        // If direction.x > 0, then order by x DESCENDING (-gridPosition.x)
        // If direction.x < 0, then order by x ASCENDING
        // If direction.z > 0, then order by z DESCENDING
        // If direction.z < 0, then order by z ASCENDING

        gridPositions = gridPositions.OrderBy(gridPosition =>
        {
            return direction.x switch
            {
                > 0 => -gridPosition.x,
                < 0 => +gridPosition.x,
                _ => direction.z switch
                {
                    > 0 => -gridPosition.z,
                    < 0 => +gridPosition.z,
                    _ => 0
                }
            };
        }).ToList();
        
        gridPositions = gridPositions.Select(gridPosition =>
        {
            Debug.Log($"Moving grid position {gridPosition}");
            
            GridPosition toGridPos = gridPosition + direction;
            
            LevelGrid.Instance.MoveGridElementGridPos(this, gridPosition, gridPosition + direction);
            
            // If the grid position is the center grid position, move the center grid position
            if (centerGridPosition == gridPosition)
            {
                centerGridPosition = toGridPos;
                targetPosition = LevelGrid.Instance.GetWorldPos(toGridPos);
                isMoving = true;
            }

            return gridPosition + direction;
        }).ToList();
    }
    
    public virtual List<GridPosition> GetGridPositionsForDirection(GridPosition direction)
    {
        return gridPositions;
    }

    // public GridPosition GetCenterGridPosition() => centerGridPosition;
    // public Vector3 GetWorldPosition() => transform.position;
}