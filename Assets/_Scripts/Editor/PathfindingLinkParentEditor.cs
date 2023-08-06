using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingLinkMonoBehaviourParent))]
public class PathfindingLinkParentEditor : Editor
{
    private void OnSceneGUI()
    {
        // Get the target of the editor (parent script)
        PathfindingLinkMonoBehaviourParent pathfindingLinkMonoBehaviour = (PathfindingLinkMonoBehaviourParent) target;

        // Get the transform of the parent object
        Transform parentTransform = pathfindingLinkMonoBehaviour.transform;

        // Loop through all the children of the parent object
        foreach (Transform transform in parentTransform)
        {
            // Get the PathfindingLinkMonoBehaviour script of the child object
            PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviourChild = transform.GetComponent<PathfindingLinkMonoBehaviour>();
        
            // Move the position handles
            PathfindingLinkEditor.MovePositionHandles(pathfindingLinkMonoBehaviourChild);
        }
    }
}
