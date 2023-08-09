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
    protected bool isMoving = false;

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
    
    //public bool IsCenterGridPosition(GridPosition gridPosition) => gridPosition == centerGridPosition;
    
    public List<GridPosition> GetGridPositions() { return gridPositions; }
    
    // public GridPosition GetGridPosition() => gridPosition;
    // public Vector3 GetWorldPosition() => transform.position;
}