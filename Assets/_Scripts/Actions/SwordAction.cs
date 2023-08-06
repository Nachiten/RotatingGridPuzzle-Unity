using System;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event Action OnAnySwordHit;

    public event Action OnSwordActionStarted;
    public event Action OnSwordActionCompleted;

    private int maxSwordDistance = 1;

    private SwordState swordState;
    private float stateTimer;
    private Unit targetUnit;

    private enum SwordState
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit
    }

    private void Update()
    {
        if (!isActive)
            return;

        stateTimer -= Time.deltaTime;

        switch (swordState)
        {
            case SwordState.SwingingSwordBeforeHit:
                const float rotationSpeed = 10f;
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

                transform.forward = Vector3.Slerp(transform.forward, aimDirection, Time.deltaTime * rotationSpeed);
                break;

            case SwordState.SwingingSwordAfterHit:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        if (stateTimer <= 0) NextState();
    }

    private void NextState()
    {
        switch (swordState)
        {
            case SwordState.SwingingSwordBeforeHit:
                swordState = SwordState.SwingingSwordAfterHit;
                const float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(85);
                OnAnySwordHit?.Invoke();
                break;

            case SwordState.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke();
                ActionComplete();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPos(gridPosition);

        swordState = SwordState.SwingingSwordBeforeHit;
        const float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke();

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidGridPositionsForAction()
    {
        List<GridPosition> validActionGridPositionList = new();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
        {
            GridPosition possibleGridPosition = unitGridPosition + new GridPosition(x, z, 0);

            // Grid pos is invalid, or doesn't have any unit
            if (!LevelGrid.Instance.GridPosIsValid(possibleGridPosition))
                continue;

            // Grid pos has an unit
            if (!LevelGrid.Instance.GridPosHasAnyUnit(possibleGridPosition))
                continue;

            Unit theTargetUnit = LevelGrid.Instance.GetUnitAtGridPos(possibleGridPosition);

            // Both units are on the same 'team'
            if (theTargetUnit.IsEnemy() == unit.IsEnemy())
                continue;

            validActionGridPositionList.Add(possibleGridPosition);
        }

        return validActionGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction(gridPosition, 200);
    }

    public int GetMaxSwordDistance()
    {
        return maxSwordDistance;
    }
}