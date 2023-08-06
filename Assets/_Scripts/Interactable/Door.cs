using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;

    private GridPosition gridPosition;
    private Animator animator;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    private Action onInteractComplete;
    private float timer;
    private bool isActive;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPos(gridPosition, this);

        SetGidPosWalkable(isOpen);
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

    public void Interact(Action onInteractionCompleteAction)
    {
        isActive = true;
        timer = 0.5f;
        onInteractComplete = onInteractionCompleteAction;
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        SetGidPosWalkable(isOpen);
        animator.SetBool(IsOpen, isOpen);
    }

    private void SetGidPosWalkable(bool isWalkable)
    {
        Pathfinding.Instance.SetGridPosIsWalkable(gridPosition, isWalkable);
    }
}