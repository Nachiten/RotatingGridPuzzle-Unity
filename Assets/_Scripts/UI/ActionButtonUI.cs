using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textUI;
    [SerializeField] private Button button;
    [SerializeField] private Image selectedImage;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction newBaseAction)
    {
        baseAction = newBaseAction;

        textUI.text = newBaseAction.GetActionName().ToUpper();

        button.onClick.AddListener(() => { UnitActionSystem.Instance.SetSelectedAction(newBaseAction); });
    }

    public void UpdateSelectedVisual()
    {
        selectedImage.enabled = isSelectedBaseAction();
    }

    private bool isSelectedBaseAction()
    {
        return UnitActionSystem.Instance.GetSelectedAction() == baseAction;
    }
}