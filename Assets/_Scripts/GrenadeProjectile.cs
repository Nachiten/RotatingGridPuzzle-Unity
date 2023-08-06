using System;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event Action OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private Action OnGrenadeHit;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update()
    {
        const float moveSpeed = 15f;

        // Calculate new position
        Vector3 newPositionXZ = Vector3.MoveTowards(positionXZ, targetPosition, moveSpeed * Time.deltaTime);
        positionXZ = newPositionXZ;

        // Calculate distance normalized
        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        // Calculate new Y position
        float maxHeight = totalDistance / 4f;
        float newPositionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;

        if (newPositionXZ == transform.position)
            TargetReached();
        else
            // Moving towards target
            transform.position = new Vector3(newPositionXZ.x, newPositionY, newPositionXZ.z);
    }

    private void TargetReached()
    {
        DamageArea();

        OnAnyGrenadeExploded?.Invoke();

        trailRenderer.transform.parent = null;

        Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1, Quaternion.identity);
        Destroy(gameObject);

        OnGrenadeHit?.Invoke();
    }

    private void DamageArea()
    {
        const float damageRadius = 4f;
        Collider[] colliders = Physics.OverlapSphere(targetPosition, damageRadius);

        foreach (Collider unitCollider in colliders)
        {
            if (unitCollider.TryGetComponent(out Unit targetUnit))
                targetUnit.Damage(30);

            if (unitCollider.TryGetComponent(out DestructibleCrate destructibleCrate))
                destructibleCrate.Damage();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action OnGrenadeHitAction)
    {
        OnGrenadeHit = OnGrenadeHitAction;
        targetPosition = LevelGrid.Instance.GetWorldPos(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}