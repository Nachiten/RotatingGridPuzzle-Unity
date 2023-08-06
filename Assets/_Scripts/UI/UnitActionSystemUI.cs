using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TMP_Text actionsPointsText;

    private List<ActionButtonUI> actionButtonUIs;

    private void Awake()
    {
        actionButtonUIs = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged;
        //UnitActionSystem.Instance.OnActionStarted += OnActionStarted;
        //TurnSystem.Instance.OnTurnChanged += OnTurnChanged;

        Unit.OnAnyActionPointsChanged += OnAnyActionPointsChanged;

        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }

    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform) Destroy(buttonTransform.gameObject);

        actionButtonUIs.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        BaseAction[] actions = selectedUnit.GetActions();

        foreach (BaseAction baseAction in actions)
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);

            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIs.Add(actionButtonUI);
        }
    }

    private void OnSelectedUnitChanged(Unit unit)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void OnSelectedActionChanged(BaseAction obj)
    {
        UpdateSelectedVisual();
    }

    private void OnActionStarted()
    {
        UpdateActionPoints();
    }

    private void OnTurnChanged()
    {
        UpdateActionPoints();
    }

    private void OnAnyActionPointsChanged()
    {
        UpdateActionPoints();
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIs) actionButtonUI.UpdateSelectedVisual();
    }

    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        actionsPointsText.text = $"Action Points: {selectedUnit.GetActionPoints()}";
    }
}