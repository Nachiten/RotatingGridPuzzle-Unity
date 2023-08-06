using System;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    public static event Action<DestructibleCrate> OnAnyDestroyed;

    [SerializeField] private Transform crateDestroyedPrefab;
    
    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPos(transform.position);
    }

    public void Damage()
    {
        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

        ApplyExplosionToChildren(crateDestroyedTransform, 150f, transform.position, 10f);

        Destroy(gameObject);

        OnAnyDestroyed?.Invoke(this);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explisionPosition,
        float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidbody))
                childRigidbody.AddExplosionForce(explosionForce, explisionPosition, explosionRange);

            ApplyExplosionToChildren(child, explosionForce, explisionPosition, explosionRange);
        }
    }
}