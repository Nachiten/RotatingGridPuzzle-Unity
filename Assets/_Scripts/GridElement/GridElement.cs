using System;
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
        OrderGridPositionsForDirection(direction);
        ExecuteGridPositionsMovement(direction);
    }

    private void ExecuteGridPositionsMovement(GridPosition direction)
    {
        // Update all grid positions and move the center grid position
        gridPositions = gridPositions.Select(gridPosition =>
        {
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

    private void OrderGridPositionsForDirection(GridPosition direction)
    {
        // Order the gridPositions depending on direction
        // If direction.x > 0, then order by x DESCENDING (-gridPosition.x)
        // So that when you move, the GridPositions to the side where we are moving to are moved first
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
    }

    public virtual List<GridPosition> GetGridPositionsForDirection(GridPosition direction)
    {
        return gridPositions;
    }

    // public GridPosition GetCenterGridPosition() => centerGridPosition;
    // public Vector3 GetWorldPosition() => transform.position;

    protected bool Equals(GridElement other)
    {
        return base.Equals(other) && centerGridPosition.Equals(other.centerGridPosition);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((GridElement)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), centerGridPosition);
    }
    
    public override string ToString()
    {
        return $"GridElement: {centerGridPosition}";
    }
}