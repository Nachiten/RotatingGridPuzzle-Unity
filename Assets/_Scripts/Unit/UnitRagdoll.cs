using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);

        Vector3 randomDir = new(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        ApplyExplisionToRagdoll(ragdollRootBone, 250f, transform.position + randomDir, 10f);
    }

    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);

            if (cloneChild == null)
                continue;

            cloneChild.position = child.position;
            cloneChild.rotation = child.rotation;

            MatchAllChildTransforms(child, cloneChild);
        }
    }

    private void ApplyExplisionToRagdoll(Transform root, float explosionForce, Vector3 explisionPosition,
        float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidbody))
                childRigidbody.AddExplosionForce(explosionForce, explisionPosition, explosionRange);

            ApplyExplisionToRagdoll(child, explosionForce, explisionPosition, explosionRange);
        }
    }
}