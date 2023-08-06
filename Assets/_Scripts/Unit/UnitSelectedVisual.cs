using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnUnitSelected;

        UpdateVisual(UnitActionSystem.Instance.GetSelectedUnit());
    }

    private void OnUnitSelected(Unit newUnit)
    {
        UpdateVisual(newUnit);
    }

    private void UpdateVisual(Unit newUnit)
    {
        meshRenderer.enabled = newUnit == unit;
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= OnUnitSelected;
    }
}