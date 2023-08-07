using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private bool isPlayer = false;
    private GridPosition gridPosition;
    
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.AddUnitAtGridPos(gridPosition, this);
    }

    private void Update()
    {
        HandlePlayerMovement();
        HandleGridPosChange();
    }

    private void HandleGridPosChange()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(transform.position);

        if (newGridPosition == gridPosition)
            return;

        // Unit changed Grid Position
        GridPosition oldGridPosition = gridPosition;
        gridPosition = newGridPosition;

        LevelGrid.Instance.MoveUnitGridPos(this, oldGridPosition, newGridPosition);
    }

    private void HandlePlayerMovement()
    {
        if (!isPlayer)
            return;
        
        // Move according to InputManager GetPlayerMovementVector vector
        Vector2 inputMoveDir = InputManager.Instance.GetPlayerMovementVector();

        if (inputMoveDir == Vector2.zero)
            return;
        
        Vector3 moveDir = new Vector3(inputMoveDir.x * LevelGrid.Instance.GetCellSize(), 0f, inputMoveDir.y * LevelGrid.Instance.GetCellSize());
        Vector3 newPosition = transform.position + moveDir;
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(newPosition);
        
        UnitMovement.TryMoveUnit(gridPosition, newGridPosition);
    }
    
    public void MoveUnitToGridPosition(GridPosition _gridPosition)
    {
        transform.position = LevelGrid.Instance.GetWorldPos(_gridPosition);
    }

    public GridPosition GetGridPosition() => gridPosition;
    public Vector3 GetWorldPosition() => transform.position;
}