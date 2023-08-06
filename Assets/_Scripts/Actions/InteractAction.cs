using System;
using System.Collections.Generic;

public class InteractAction : BaseAction
{
    private const int maxInteractDistance = 1;

    public override string GetActionName()
    {
        return "Interact";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        LevelGrid.Instance.GetInteractableAtGridPos(gridPosition).Interact(OnInteractComplete);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidGridPositionsForAction()
    {
        List<GridPosition> validActionGridPositionList = new();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
        {
            GridPosition possibleGridPosition = unitGridPosition + new GridPosition(x, z, 0);

            // Grid pos is invalid
            if (!LevelGrid.Instance.GridPosIsValid(possibleGridPosition))
                continue;

            // There is no door at this pos
            if (LevelGrid.Instance.GetInteractableAtGridPos(possibleGridPosition) == null)
                continue;

            validActionGridPositionList.Add(possibleGridPosition);
        }

        return validActionGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction(gridPosition, 0);
    }

    private void OnInteractComplete()
    {
        ActionComplete();
    }
}