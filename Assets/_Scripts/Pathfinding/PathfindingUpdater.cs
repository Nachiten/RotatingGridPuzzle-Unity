using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleCrate.OnAnyDestroyed += OnAnyDestroyed;
    }

    private void OnAnyDestroyed(DestructibleCrate crate)
    {
        Pathfinding.Instance.SetGridPosIsWalkable(crate.GetGridPosition(), true);
    }
}