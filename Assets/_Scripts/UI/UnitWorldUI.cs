using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text actionsPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += OnAnyActionPointsChanged;
        healthSystem.OnDamaged += OnDamaged;

        UpdateActionsPointsText();
        UpdateHealthBar();
    }

    private void OnDamaged()
    {
        UpdateHealthBar();
    }

    private void OnAnyActionPointsChanged()
    {
        UpdateActionsPointsText();
    }

    private void UpdateActionsPointsText()
    {
        actionsPointsText.text = unit.GetActionPoints().ToString();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }
}