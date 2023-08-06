using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TMP_Text gridText;

    private object gridObject;

    protected virtual void Update()
    {
        gridText.text = gridObject.ToString();
    }

    public virtual void SetGridObject(object gridObject)
    {
        this.gridObject = gridObject;
    }
}