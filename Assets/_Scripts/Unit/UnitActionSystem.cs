using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event Action<Unit> OnSelectedUnitChanged;
    public event Action<BaseAction> OnSelectedActionChanged;
    public event Action<bool> OnBusyChanged;
    public event Action OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of UnitActionSystem found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        mainCamera = Camera.main;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {
        //Debug.Log(LevelGrid.Instance.GetGridPos(MouseWorld.GetPosition()));
        
        // If we are already busy with an action, or if the mouse is over an UI element, do nothing
        if (isBusy || EventSystem.current.IsPointerOverGameObject())
            return;

        // On enemy turn, do nothing
        if (!TurnSystem.Instance.IsPlayerTurn())
            return;

        // Handles unit selection if possible
        if (TryHandleUnitSelection())
            return;

        // Handles selected action if possible
        HandleSelectedAction();
    }

    private bool TryHandleUnitSelection()
    {
        // Only do this on mouse click
        if (!InputManager.Instance.IsMouseButtonDownThisFrame())
            return false;

        Ray ray = mainCamera.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());

        // Select an unit with raycast
        if (!Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask) ||
            !raycastHit.transform.TryGetComponent(out Unit unit))
            return false;

        // This unit is already selected, or unit is enemy
        if (unit == selectedUnit || unit.IsEnemy())
            return false;

        // Select new unit if raycast did hit
        SetSelectedUnit(unit);
        return true;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());

        OnSelectedUnitChanged?.Invoke(unit);
    }

    public void SetSelectedAction(BaseAction action)
    {
        selectedAction = action;

        OnSelectedActionChanged?.Invoke(action);
    }

    private void HandleSelectedAction()
    {
        // Only do this on mouse click
        if (!InputManager.Instance.IsMouseButtonDownThisFrame())
            return;

        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPos(MouseWorld.GetPositionOnlyHitVisible());

        // Check if grid position is valid for the action
        if (!selectedAction.GridPositionIsValidForAction(mouseGridPosition))
            return;

        // Spend the points to take this action
        if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            return;

        SetBusy();
        selectedAction.TakeAction(mouseGridPosition, ClearBusy);

        OnActionStarted?.Invoke();
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(true);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(false);
    }
}