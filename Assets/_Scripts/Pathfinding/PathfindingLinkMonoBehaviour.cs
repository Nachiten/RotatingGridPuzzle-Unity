using UnityEngine;

public class PathfindingLinkMonoBehaviour : MonoBehaviour
{
    public Vector3 linkPositionA;
    public Vector3 linkPositionB;
    
    public PathfindingLink GetPathfindingLink()
    {
        return new PathfindingLink(
            LevelGrid.Instance.GetGridPos(linkPositionA), 
            LevelGrid.Instance.GetGridPos(linkPositionB));
    }
}
