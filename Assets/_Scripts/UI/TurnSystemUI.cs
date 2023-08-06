using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TMP_Text currentTurnText;
    [SerializeField] private GameObject enemyTurnVisual;

    private void Start()
    {
        endTurnButton.onClick.AddListener(EndTurn);

        TurnSystem.Instance.OnTurnChanged += OnTurnChanged;

        UpdateVisuals();
    }

    private void OnTurnChanged()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void EndTurn()
    {
        TurnSystem.Instance.NextTurn();
    }

    private void UpdateTurnText()
    {
        int turnNumber = TurnSystem.Instance.GetTurnNumber();
        currentTurnText.text = $"TURN {turnNumber}";
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisual.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}