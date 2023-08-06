using System;
using UnityEngine;

public class BarrelInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform barrelDestroyedPrefab;
    [SerializeField] private GameObject visualGameObject;

    private GridPosition gridPosition;
    private Action onInteractionComplete;
    private bool isActive;
    private float timer;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPos(gridPosition, this);
        Pathfinding.Instance.SetGridPosIsWalkable(gridPosition, false);
    }

    private void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isActive = false;

            LevelGrid.Instance.ClearInteractableAtGridPos(gridPosition);
            Pathfinding.Instance.SetGridPosIsWalkable(gridPosition, true);

            Destroy(gameObject);
            onInteractionComplete();
        }
    }

    public void Interact(Action onInteractionCompleteAction)
    {
        onInteractionComplete = onInteractionCompleteAction;
        isActive = true;
        timer = .5f;

        visualGameObject.SetActive(false);

        Transform barrelDestroyedTransform = Instantiate(barrelDestroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToChildren(barrelDestroyedTransform, 250f, transform.position, 10f);
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition,
        float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}