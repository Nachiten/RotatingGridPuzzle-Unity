using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    [Header("Speeds")] [SerializeField] private float totalSpin = 90;

    private float totalSpinAmount;

    private void Update()
    {
        if (!isActive)
            return;

        float spinSpeed = 360 * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinSpeed, 0);

        totalSpinAmount += spinSpeed;

        if (totalSpinAmount >= totalSpin)
            ActionComplete();
    }

    public override void TakeAction(GridPosition gridPosition, Action onCompleteAction)
    {
        totalSpinAmount = 0;

        ActionStart(onCompleteAction);
    }

    public override List<GridPosition> GetValidGridPositionsForAction()
    {
        return new List<GridPosition> {unit.GetGridPosition()};
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction(gridPosition, 0);
    }

    public override string GetActionName()
    {
        return "Spin";
    }
}