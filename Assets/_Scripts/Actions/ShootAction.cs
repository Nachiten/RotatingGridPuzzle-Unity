using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    [Range(0, 100)] [SerializeField] private int damage = 40;
    [SerializeField] private LayerMask obstaclesLayerMask;
    private int maxShootDistance = 20;

    // Params: Target unit, shooting unit
    public event Action<Unit, Unit> OnShoot;

    public static event Action<Unit, Unit> OnAnyShoot;

    private Unit targetUnit;
    private bool canShootBullet;

    private float stateTimer;
    private ShootState shootState;

    private enum ShootState
    {
        Aiming,
        Shooting,
        Cooloff
    }

    private void Update()
    {
        if (!isActive)
            return;

        stateTimer -= Time.deltaTime;

        switch (shootState)
        {
            case ShootState.Aiming:
                const float rotationSpeed = 10f;
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                aimDirection.y = 0;

                transform.forward = Vector3.Slerp(transform.forward, aimDirection, Time.deltaTime * rotationSpeed);
                break;
            case ShootState.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }

                break;
            case ShootState.Cooloff:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (stateTimer <= 0) NextState();
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(targetUnit, unit);
        OnShoot?.Invoke(targetUnit, unit);

        targetUnit.Damage(damage);
    }

    private void NextState()
    {
        switch (shootState)
        {
            case ShootState.Aiming:
                shootState = ShootState.Shooting;
                const float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;

            case ShootState.Shooting:
                shootState = ShootState.Cooloff;
                const float coolOffStateTime = 0.5f;
                stateTimer = coolOffStateTime;
                break;

            case ShootState.Cooloff:
                ActionComplete();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onCompleteAction)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPos(gridPosition);

        shootState = ShootState.Aiming;
        const float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onCompleteAction);
    }

    public override List<GridPosition> GetValidGridPositionsForAction()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidGridPositionsForAction(unitGridPosition);
    }

    private List<GridPosition> GetValidGridPositionsForAction(GridPosition unitGridPosition)
    {
        List<GridPosition> validActionGridPositionList = new();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        for (int z = -maxShootDistance; z <= maxShootDistance; z++)
        for (int floor = -maxShootDistance; floor <= maxShootDistance; floor++)
        {
            GridPosition possibleGridPosition = unitGridPosition + new GridPosition(x, z, floor);

            // Grid pos is invalid, or doesn't have any unit
            if (!LevelGrid.Instance.GridPosIsValid(possibleGridPosition) ||
                !LevelGrid.Instance.GridPosHasAnyUnit(possibleGridPosition))
                continue;
            
            // Grid pos is too far away
            if (unitGridPosition.Distance(possibleGridPosition) > maxShootDistance)
                continue;

            Unit _targetUnit = LevelGrid.Instance.GetUnitAtGridPos(possibleGridPosition);

            if (_targetUnit.IsEnemy() == unit.IsEnemy())
            {
                // Both Units on same 'team'
                continue;
            }

            Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPos(unitGridPosition);
            Vector3 shootDir = (_targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

            const float unitShoulderHeight = 1.7f;
            float distanceRaycast = Vector3.Distance(unitWorldPosition, _targetUnit.GetWorldPosition());
            
            // Draw debug ray to see if there is any obstacle in the way
            Debug.DrawRay(unitWorldPosition + Vector3.up * unitShoulderHeight, shootDir * distanceRaycast, Color.red, 10f);
            
            if (Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    distanceRaycast,
                    obstaclesLayerMask))
            {
                // Blocked by an Obstacle
                continue;
            }

            validActionGridPositionList.Add(possibleGridPosition);
        }

        return validActionGridPositionList;
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit theTargetUnit = LevelGrid.Instance.GetUnitAtGridPos(gridPosition);

        float unitHealth = theTargetUnit.GetHealthNormalized();

        return new EnemyAIAction(gridPosition, 100 + Mathf.RoundToInt((1 - unitHealth) * 100f));
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidGridPositionsForAction(gridPosition).Count;
    }
}