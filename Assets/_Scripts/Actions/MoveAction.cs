using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event Action OnStartMoving;
    public event Action OnStopMoving;
    public event Action<GridPosition, GridPosition> OnChangeFloorStarted;

    [Header("Speeds")] [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Config")] [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionsPath;
    private int currentPositionIndex;
    private Vector3 targetPosition;
    private bool isChangingFloors;
    private float differentFloorsTeleportTimer;
    
    private const float differentFloorsTeleportTimerMax = 0.5f;

    private void Update()
    {
        if (!isActive)
            return;

        HandleWalk();
    }
    
    private void HandleWalk()
    {
        if (isChangingFloors)
        {
            // Rotate towards ladder for changing floors
            Vector3 targetSameFloorPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized;
            transform.forward = Vector3.Slerp(transform.forward, rotateDirection, Time.deltaTime * rotationSpeed);
            
            differentFloorsTeleportTimer -= Time.deltaTime;
            if (differentFloorsTeleportTimer <= 0f)
            {
                isChangingFloors = false;
                transform.position = targetPosition;
            }
        }
        else
        {
            // Rotate towards target
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);

            // Calculate new position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        // We are walking if we didn't reach our target
        if (transform.position != targetPosition) 
            return;
        
        currentPositionIndex++;

        // Check if we reached the end of the path
        if (currentPositionIndex >= positionsPath.Count)
        {
            OnStopMoving?.Invoke();
            ActionComplete();
            return;
        }

        targetPosition = positionsPath[currentPositionIndex];
        GridPosition targetGridPosition = LevelGrid.Instance.GetGridPos(targetPosition);
        GridPosition unitGridPosition = LevelGrid.Instance.GetGridPos(transform.position);

        if (targetGridPosition.floor == unitGridPosition.floor) 
            return;
        
        isChangingFloors = true;
        differentFloorsTeleportTimer = differentFloorsTeleportTimerMax;
        OnChangeFloorStarted?.Invoke(unitGridPosition, targetGridPosition);
    }

    public override void TakeAction(GridPosition gridPosition, Action onCompleteAction)
    {
        List<GridPosition> pathGridPositions =
            Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int _);

        positionsPath = new List<Vector3>();
        foreach (GridPosition position in pathGridPositions)
            positionsPath.Add(LevelGrid.Instance.GetWorldPos(position));

        currentPositionIndex = 0;
        targetPosition = positionsPath[currentPositionIndex];

        OnStartMoving?.Invoke();

        ActionStart(onCompleteAction);
    }

    public override List<GridPosition> GetValidGridPositionsForAction()
    {
        List<GridPosition> validActionGridPositionList = new();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
        for (int floor= -maxMoveDistance; floor <= maxMoveDistance; floor++)
        {
            GridPosition possibleGridPosition = unitGridPosition + new GridPosition(x, z, floor);

            bool samePosition = possibleGridPosition == unitGridPosition;

            // If the position is invalid, has any unit, or is the same position
            if (!LevelGrid.Instance.GridPosIsValid(possibleGridPosition) || samePosition ||
                LevelGrid.Instance.GridPosHasAnyUnit(possibleGridPosition))
                continue;

            // If the unit can not reach this position
            if (!Pathfinding.Instance.GridPosIsWalkable(possibleGridPosition) ||
                !Pathfinding.Instance.HasPath(unitGridPosition, possibleGridPosition))
                continue;

            const int pathfindingDistanceMultiplier = 10;
            int pathLength = Pathfinding.Instance.GetPathLength(unitGridPosition, possibleGridPosition);

            // If the path is too long
            if (pathLength > maxMoveDistance * pathfindingDistanceMultiplier)
                continue;

            validActionGridPositionList.Add(possibleGridPosition);
        }

        return validActionGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override int GetActionCost()
    {
        return 2;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPos = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction(gridPosition, targetCountAtGridPos * 10);
    }
}