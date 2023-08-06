using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform grenadeProjectilePrefab;

    private int maxThrowDistance = 7;

    public override string GetActionName()
    {
        return "Grenade";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform =
            Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        grenadeProjectileTransform.GetComponent<GrenadeProjectile>().Setup(gridPosition, OnGrenadeHit);

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidGridPositionsForAction()
    {
        List<GridPosition> validActionGridPositionList = new();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
        {
            GridPosition possibleGridPosition = unitGridPosition + new GridPosition(x, z, 0);

            // Grid pos is invalid, or doesn't have any unit
            if (!LevelGrid.Instance.GridPosIsValid(possibleGridPosition))
                continue;

            // Grid pos is too far away
            if (unitGridPosition.Distance(possibleGridPosition) > maxThrowDistance)
                continue;

            validActionGridPositionList.Add(possibleGridPosition);
        }

        return validActionGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction(gridPosition, 0);
    }

    private void OnGrenadeHit()
    {
        ActionComplete();
    }
}