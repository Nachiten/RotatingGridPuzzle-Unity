using System;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private GridPosition gridPosition;

    private bool isGreen;

    private Action onInteractComplete;
    private float timer;
    private bool isActive;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPos(gridPosition, this);

        SetColorGreen();
    }

    private void Update()
    {
        if (!isActive)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            isActive = false;
            onInteractComplete();
        }
    }

    private void SetColorGreen()
    {
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }

    private void SetColorRed()
    {
        isGreen = false;
        meshRenderer.material = redMaterial;
    }

    public void Interact(Action onInteractionCompleteAction)
    {
        onInteractComplete = onInteractionCompleteAction;
        isActive = true;
        timer = 0.5f;

        if (isGreen)
            SetColorRed();
        else
            SetColorGreen();
    }
}