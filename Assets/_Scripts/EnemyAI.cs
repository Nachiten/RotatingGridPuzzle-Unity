using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private State state;
    private float timer;
    
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }
    
    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
            return;

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                // Does nothing
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    if (TryTakeEnemyAIAction(EndEnemyAction))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                        state = State.WaitingForEnemyTurn;
                    }
                }

                break;
            case State.Busy:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnTurnChanged()
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyActionComplete)
    {
        List<Unit> enemyUnits = UnitManager.Instance.GetEnemyUnits();

        foreach (Unit enemyUnit in enemyUnits)
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyActionComplete))
                return true;

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetActions())
        {
            if (!enemyUnit.CanSpendPointsToTakeAction(baseAction))
                // Enemy can't afford this action
                continue;

            EnemyAIAction enemyAIAction = baseAction.GetBestEnemyAIAction();

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = enemyAIAction;
                bestAction = baseAction;
            }
            else if (!(enemyAIAction == null || enemyAIAction.actionValue <= bestEnemyAIAction.actionValue))
            {
                bestEnemyAIAction = enemyAIAction;
                bestAction = baseAction;
            }
        }

        // There is no action, or cannot be taken
        if (bestEnemyAIAction == null || !enemyUnit.TrySpendActionPointsToTakeAction(bestAction))
            return false;

        // Action is taken
        bestAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyActionComplete);
        return true;
    }

    private void EndEnemyAction()
    {
        timer = .5f;

        state = State.TakingTurn;
    }
}