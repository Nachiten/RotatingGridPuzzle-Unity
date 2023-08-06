using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    private void Start()
    {
        SetActive(false);
        UnitActionSystem.Instance.OnBusyChanged += SetActive;
    }

    private void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}