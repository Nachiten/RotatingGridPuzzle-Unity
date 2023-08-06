using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingLinkMonoBehaviour))]
public class PathfindingLinkEditor : Editor
{
    private void OnSceneGUI()
    {
        PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour = (PathfindingLinkMonoBehaviour) target;
        
        MovePositionHandles(pathfindingLinkMonoBehaviour);
    }

    public static void MovePositionHandles(PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour)
    {
        EditorGUI.BeginChangeCheck();
        Vector3 newLinkPositionA = Handles.PositionHandle(pathfindingLinkMonoBehaviour.linkPositionA, Quaternion.identity);
        Vector3 newLinkPositionB = Handles.PositionHandle(pathfindingLinkMonoBehaviour.linkPositionB, Quaternion.identity);

        if (!EditorGUI.EndChangeCheck()) 
            return;
        
        Undo.RecordObject(pathfindingLinkMonoBehaviour, "Change Link Positions");
        pathfindingLinkMonoBehaviour.linkPositionA = newLinkPositionA;
        pathfindingLinkMonoBehaviour.linkPositionB = newLinkPositionB;
    }
}
