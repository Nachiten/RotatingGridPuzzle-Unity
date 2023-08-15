using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private Transform gridSystemVisualSingleParent;
    [SerializeField] private LayerMask objectivesLayerMask;

    private GridSystemVisualSingle[,,] gridSystemVisualSingleArray;

    private void Start()
    {
        int width = LevelGrid.Instance.GetWidth();
        int height = LevelGrid.Instance.GetHeight();
        int totalFloors = LevelGrid.Instance.GetTotalFloors();

        gridSystemVisualSingleArray = new GridSystemVisualSingle[width, height, totalFloors];

        for (int x = 0; x < width; x++)
        for (int z = 0; z < height; z++)
        for (int floor = 0; floor < totalFloors; floor++)
        {
            Vector3 worldPos = LevelGrid.Instance.GetWorldPos(new GridPosition(x, z, floor));
            const float raycastOffsetDistance = 1f;
            
            // Check if there is already an objective on this grid position
            bool objectivesRaycast = Physics.Raycast(
                worldPos + Vector3.up * raycastOffsetDistance,
                Vector3.down,
                raycastOffsetDistance * 2,
                objectivesLayerMask);

            if (objectivesRaycast) 
                continue;
            
            Transform gridSystemVisual =
                Instantiate(gridSystemVisualSinglePrefab, 
                    worldPos, 
                    Quaternion.identity, 
                    gridSystemVisualSingleParent);
                
            gridSystemVisualSingleArray[x, z, floor] = gridSystemVisual.GetComponent<GridSystemVisualSingle>();
        }
    }
}