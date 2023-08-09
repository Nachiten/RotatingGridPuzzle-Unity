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

        // If the player is moving, teletransport it to the previous target position
        if (isMoving)
            transform.position = targetPosition;

        // Calculate new position
        Vector3 moveDir = new Vector3(inputMoveDir.x * LevelGrid.Instance.GetCellSize(), 0f, inputMoveDir.y * LevelGrid.Instance.GetCellSize());
        Vector3 newPosition = transform.position + moveDir;
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(newPosition);
        
        // TODO - Se chequean todas las grid positions que tiene el objeto ACTUAL, tambien hay que chequear
        // todas las que pueda llegar a tener el objeto A MOVER. Error de logica
        LevelGrid.Instance.TryMoveGridElements(gridPositions, newGridPosition - centerGridPosition);
        
        // Pasos del try move grid elements:
        // 
    }
}
