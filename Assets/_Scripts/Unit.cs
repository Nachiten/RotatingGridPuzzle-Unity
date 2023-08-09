using UnityEngine;

public class Unit : MonoBehaviour
{
    private float moveSpeed = 24f;
    [SerializeField] private bool isPlayer = false;
    private GridPosition gridPosition;
    private Vector3 targetPosition = Vector3.positiveInfinity;
    private bool isMoving = false;
    
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.AddUnitAtGridPos(gridPosition, this);
    }

    private void Update()
    {
        HandlePlayerInput();
        HandlePlayerMovement();
    }

    private void HandlePlayerInput()
    {
        if (!isPlayer)
            return;
        
        // Move according to InputManager GetPlayerMovementVector vector
        Vector2 inputMoveDir = InputManager.Instance.GetPlayerMovementVector();

        // There is no input
        if (inputMoveDir == Vector2.zero)
            return;

        // If the unit is moving, teletransport it to the previous target position
        if (isMoving && isPlayer)
            transform.position = targetPosition;

        // Calculate new position
        Vector3 moveDir = new Vector3(inputMoveDir.x * LevelGrid.Instance.GetCellSize(), 0f, inputMoveDir.y * LevelGrid.Instance.GetCellSize());
        Vector3 newPosition = transform.position + moveDir;
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(newPosition);
        
        LevelGrid.Instance.TryMoveUnit(gridPosition, newGridPosition);
    }

    private void HandlePlayerMovement()
    {
        if (!isMoving)
            return; 
        
        // Calculate new position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        if (transform.position == targetPosition)
            isMoving = false;
    }

    public void MoveUnitToGridPosition(GridPosition toGridPosition)
    {
        targetPosition = LevelGrid.Instance.GetWorldPos(toGridPosition);
        isMoving = true;
        SetGridPos(toGridPosition);
    }
    
    private void SetGridPos(GridPosition newGridPosition)
    {
        // Unit changed Grid Position
        GridPosition oldGridPosition = gridPosition;
        gridPosition = newGridPosition;

        LevelGrid.Instance.MoveUnitGridPos(this, oldGridPosition, newGridPosition);
    }
    
    public GridPosition GetGridPosition() => gridPosition;
    public Vector3 GetWorldPosition() => transform.position;
}