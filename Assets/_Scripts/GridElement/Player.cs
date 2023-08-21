using UnityEngine;

public class Player : GridElement
{
    // Cooldown
    private float moveCooldownTimer;
    private const float moveCooldownMax = 0.2f;
    private bool isInCooldown;

    // Movement
    private GridPosition moveDirection = GridPosition.Zero;
    private Vector2 inputMoveDir = Vector2.zero;
    
    protected override void Update()
    {
        base.Update();
        
        HandlePlayerInput();
        HandlePlayerMovement();
    }

    private void HandlePlayerInput()
    {
        inputMoveDir = InputManager.Instance.GetPlayerMovementVector();
    }

    private void HandlePlayerMovement()
    {
        if (ShouldMove())
            MovePlayer();
    }

    private bool ShouldMove()
    {
        // If there is no input, stop cooldown and dont move
        if (inputMoveDir == Vector2.zero)
        {
            StopCooldown();
            return false;
        }

        // If the player is not in cooldown, reset cooldown and move
        if (!isInCooldown)
        {
            ResetCooldown();
            return true;
        }

        // If move direction changed, reset cooldown and move
        if (MoveDirectionChanged())
        {
            ResetCooldown();
            return true;
        }

        moveCooldownTimer -= Time.deltaTime;
        // If timer is not finsihed, dont move
        if (moveCooldownTimer > 0f)
            return false;
        
        // If timer is finished, reset cooldown and move
        ResetCooldown();
        return true;
    }

    private void StopCooldown()
    {
        isInCooldown = false;
    }

    private void ResetCooldown()
    {
        isInCooldown = true;
        moveCooldownTimer = moveCooldownMax;
    }

    private bool MoveDirectionChanged()
    {
        Vector3 moveDir = new(inputMoveDir.x * LevelGrid.Instance.GetCellSize(), 0f, inputMoveDir.y * LevelGrid.Instance.GetCellSize());
        Vector3 newPosition = transform.position + moveDir;
        
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(newPosition);
        GridPosition newMoveDirection = newGridPosition - centerGridPosition;
        
        Debug.Log("New move direction: " + newMoveDirection);

        return newMoveDirection != moveDirection && newMoveDirection != GridPosition.Zero;
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
        
        GridElementMovement.Instance.TryMoveGridElements(GetGridPositionsForDirection(moveDirection), moveDirection);
    }
}
