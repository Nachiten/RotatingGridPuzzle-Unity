using System.Collections.Generic;
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
        if (ShouldTryToMove())
            TryMovePlayer();
    }

    private bool ShouldTryToMove()
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
        // If timer is not finished, dont move
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
        GridPosition newMoveDirection = GetMoveDirectionGridPositionForOriginPos(transform.position);
        
        // Get if there is new move direction, and if it changed
        return newMoveDirection != moveDirection && newMoveDirection != GridPosition.Zero;
    }

    private void TryMovePlayer()
    {
        CalculateNewMoveDirection();
        
        List<GridPosition> gridPositionsForDirection = GetGridPositionsForDirection(moveDirection);
        
        // Get if player can indeed move and push blocks if needed
        bool canMove = GridElementMovement.Instance.CanMoveGridElements(gridPositionsForDirection, moveDirection);

        if (!canMove) 
            return;
        
        // If the player was already moving, teleport it to the previous target position before starting the new movement
        if (isMoving)
            transform.position = targetPosition;
        
        // We MUST have checked that it can move before calling this, if not explosion
        GridElementMovement.Instance.MoveGridElements(gridPositionsForDirection, moveDirection);
    }

    private void CalculateNewMoveDirection()
    {
        // If we are moving, the origin position is the target position (as if we already finished moving), otherwise it is the current position
        Vector3 originPosition = isMoving ? targetPosition : transform.position;
        
        moveDirection = GetMoveDirectionGridPositionForOriginPos(originPosition);
    }

    private GridPosition GetMoveDirectionGridPositionForOriginPos(Vector3 originPosition)
    {
        Vector3 newPosition = originPosition + CalculateMoveDirVector();
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(newPosition);

        return newGridPosition - centerGridPosition;
    }

    private Vector3 CalculateMoveDirVector()
    {
        return new Vector3(inputMoveDir.x * LevelGrid.Instance.GetCellSize(), 0f, inputMoveDir.y * LevelGrid.Instance.GetCellSize());
    }
}
