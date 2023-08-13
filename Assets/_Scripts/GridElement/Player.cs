using UnityEngine;

public class Player : GridElement
{
    private float moveCooldownTimer;
    private const float moveCooldownMax = 0.2f;
    private bool isInCooldown;
    private GridPosition moveDirection;
    private Vector2 inputMoveDir;
    
    protected override void Update()
    {
        base.Update();
        
        HandlePlayerInput();
        HandlePlayerMovement();
    }

    private void HandlePlayerInput()
    {
        // Move according to InputManager GetPlayerMovementVector vector
        inputMoveDir = InputManager.Instance.GetPlayerMovementVector();
    }

    private void HandlePlayerMovement()
    {
        // If there is no input, reset cooldown
        if (inputMoveDir == Vector2.zero)
        {
            isInCooldown = false;
            return;
        }

        if (!isInCooldown)
        {
            MovePlayer();
            return;
        }
      
        moveCooldownTimer -= Time.deltaTime;
        if (moveCooldownTimer > 0f)
            return;

        isInCooldown = false;
        MovePlayer();
    }

    private void MovePlayer()
    {
        // If the player was already moving, teleport it to the previous target position
        if (isMoving)
            transform.position = targetPosition;

        // Calculate new position
        Vector3 moveDir = new(inputMoveDir.x * LevelGrid.Instance.GetCellSize(), 0f, inputMoveDir.y * LevelGrid.Instance.GetCellSize());
        Vector3 newPosition = transform.position + moveDir;
        
        // Calculate new grid position and direction
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(newPosition);
        moveDirection = newGridPosition - centerGridPosition;
        
        LevelGrid.Instance.TryMoveGridElements(GetGridPositionsForDirection(moveDirection), moveDirection);
        
        isInCooldown = true;
        moveCooldownTimer = moveCooldownMax;
    }
}
