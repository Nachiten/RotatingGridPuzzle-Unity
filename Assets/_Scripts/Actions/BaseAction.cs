using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static event Action<BaseAction> OnAnyActionStarted;
    public static event Action<BaseAction> OnAnyActionCompleted;

    protected bool isActive;
    protected Unit unit;

    private Action onActionComplete;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public bool GridPositionIsValidForAction(GridPosition gridPosition)
    {
        return GetValidGridPositionsForAction().Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidGridPositionsForAction();

    public virtual int GetActionCost()
    {
        return 1;
    }

    protected void ActionStart(Action onCompleteAction)
    {
        isActive = true;
        onActionComplete = onCompleteAction;

        OnAnyActionStarted?.Invoke(this);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActions = new();

        List<GridPosition> validActionGridPositionList = GetValidGridPositionsForAction();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActions.Add(enemyAIAction);
        }

        if (enemyAIActions.Count > 0)
        {
            enemyAIActions.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActions[0];

            // Get the enemyAIAction with the highest actionValue
            //return enemyAIActions.Aggregate((a, b) => a.actionValue > b.actionValue ? a : b);
        }
        else
        {
            // No possible actions
            return null;
        }
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
}