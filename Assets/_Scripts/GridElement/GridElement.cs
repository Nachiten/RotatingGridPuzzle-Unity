using UnityEngine;

public abstract class GridElement : MonoBehaviour
{
    private const float moveSpeed = 20f;
    protected GridPosition gridPosition;
    protected Vector3 targetPosition;
    protected bool isMoving = false;
    
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.AddGridElementAtGridPos(gridPosition, this);
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

    public void MoveGridElementToGridPosition(GridPosition toGridPosition)
    {
        targetPosition = LevelGrid.Instance.GetWorldPos(toGridPosition);
        isMoving = true;
        SetGridPos(toGridPosition);
    }
    
    private void SetGridPos(GridPosition newGridPosition)
    {
        // Grid element changed Grid Position
        GridPosition oldGridPosition = gridPosition;
        gridPosition = newGridPosition;

        LevelGrid.Instance.MoveGridElementGridPos(this, oldGridPosition, newGridPosition);
    }
    
    // public GridPosition GetGridPosition() => gridPosition;
    // public Vector3 GetWorldPosition() => transform.position;
}