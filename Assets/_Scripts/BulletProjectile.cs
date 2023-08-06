using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Transform bulletHitVFXPrefab;
    
    private TrailRenderer trailRenderer;
    private Vector3 targetPosition;

    private void Awake()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

        const float moveSpeed = 200f;
        transform.position += moveDir * (Time.deltaTime * moveSpeed);

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if (distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetPosition;

            trailRenderer.transform.parent = null;
            Destroy(gameObject);

            Instantiate(bulletHitVFXPrefab, targetPosition, Quaternion.identity);
        }
    }

    public void Setup(Vector3 theTargetPosition)
    {
        targetPosition = theTargetPosition;
    }
}