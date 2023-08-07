using System;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private bool isPlayer = false;
    
    private int actionPointsMax = 50;

    // [SerializeField] private bool isEnemy;
    // [SerializeField] private int actionPoints;

    // public static event Action OnAnyActionPointsChanged;
    // public static event Action<Unit> OnAnyUnitSpawned;
    // public static event Action<Unit> OnAnyUnitDead;

    private GridPosition gridPosition;

    //private BaseAction[] actions;
    // private HealthSystem healthSystem;

    private void Awake()
    {
        // healthSystem = GetComponent<HealthSystem>();
        //actions = GetComponents<BaseAction>();

        // actionPoints = actionPointsMax;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.AddUnitAtGridPos(gridPosition, this);

        // TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        // healthSystem.OnDead += OnDead;

        // OnAnyUnitSpawned?.Invoke(this);
    }

    private void Update()
    {
        HandlePlayerMovement();
        HandleGridPosChange();
    }

    private void HandleGridPosChange()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(transform.position);

        if (newGridPosition == gridPosition)
            return;

        // Unit changed Grid Position
        GridPosition oldGridPosition = gridPosition;
        gridPosition = newGridPosition;

        LevelGrid.Instance.MoveUnitGridPos(this, oldGridPosition, newGridPosition);
    }

    private void HandlePlayerMovement()
    {
        if (!isPlayer)
            return;
        
        // Move according to InputManager GetPlayerMovementVector vector
        Vector2 inputMoveDir = InputManager.Instance.GetPlayerMovementVector();

        if (inputMoveDir == Vector2.zero)
            return;
        
        Vector3 moveDir = new Vector3(inputMoveDir.x * LevelGrid.Instance.GetCellSize(), 0f, inputMoveDir.y * LevelGrid.Instance.GetCellSize());
        Vector3 newPosition = transform.position + moveDir;
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPos(newPosition);

        if(UnitMovement.Instance.TryMoveUnit(gridPosition, newGridPosition))
            MoveUnitToGridPosition(newGridPosition);
    }

    // public T GetAction<T>() where T : BaseAction
    // {
    //     return actions.OfType<T>().FirstOrDefault();
    // }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    // private void MoveUnitToPosition(Vector3 newPosition)
    // {
    //     transform.position = newPosition;
    // }
    
    public void MoveUnitToGridPosition(GridPosition _gridPosition)
    {
        transform.position = LevelGrid.Instance.GetWorldPos(_gridPosition);
    }

    // public BaseAction[] GetActions()
    // {
    //     return actions;
    // }

    // public bool TrySpendActionPointsToTakeAction(BaseAction action)
    // {
    //     if (!CanSpendPointsToTakeAction(action))
    //         return false;
    //
    //     SpendActionPoints(action.GetActionCost());
    //     return true;
    // }
    //
    // public bool CanSpendPointsToTakeAction(BaseAction action)
    // {
    //     return actionPoints >= action.GetActionCost();
    // }

    // private void SpendActionPoints(int amount)
    // {
    //     actionPoints -= amount;
    //
    //     OnAnyActionPointsChanged?.Invoke();
    // }

    // private void OnTurnChanged()
    // {
    //     bool isEnemyAndEnemyTurn = isEnemy && !TurnSystem.Instance.IsPlayerTurn();
    //     bool isPlayerAndPlayerTurn = !isEnemy && TurnSystem.Instance.IsPlayerTurn();
    //
    //     if (!(isEnemyAndEnemyTurn || isPlayerAndPlayerTurn)) return;
    //
    //     actionPoints = actionPointsMax;
    //
    //     OnAnyActionPointsChanged?.Invoke();
    // }
    //
    // private void OnDead()
    // {
    //     LevelGrid.Instance.RemoveUnitAtGridPos(gridPosition, this);
    //     Destroy(gameObject);
    //
    //     OnAnyUnitDead?.Invoke(this);
    // }

    // public int GetActionPoints()
    // {
    //     return actionPoints;
    // }
    //
    // public bool IsEnemy()
    // {
    //     return isEnemy;
    // }

    // public void Damage(int damage)
    // {
    //     healthSystem.Damage(damage);
    // }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    // public float GetHealthNormalized()
    // {
    //     return healthSystem.GetHealthNormalized();
    // }
}