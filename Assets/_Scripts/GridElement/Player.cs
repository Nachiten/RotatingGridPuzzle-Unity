using UnityEngine;

public class Player : GridElement
{
    protected override void Update()
    {
        base.Update();
        
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        // Move according to InputManager GetPlayerMovementVector vector
        Vector2 inputMoveDir = InputManager.Instance.GetPlayerMovementVector();

        // There is no input
        if (inputMoveDir == Vector2.zero)
            return;

        // If the player is moving, transport it to the previous target position
        if (isMoving)
            transform.position = targetPosition;

        // Calculate new position
        Vector3 moveDir = new(inputMoveDir.x * LevelGrid.Instance.GetCellSize(), 0f, inputMoveDir.y * LevelGrid.Instance.GetCellSize());
        Vector3 newPosition = transform.position + moveDir;
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(newPosition);
        GridPosition direction = newGridPosition - centerGridPosition;
        
        LevelGrid.Instance.TryMoveGridElements(GetGridPositionsForDirection(direction), direction);
    }
}
