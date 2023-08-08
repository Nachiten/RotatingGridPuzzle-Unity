using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private Transform gridSystemVisualSingleParent;

    private GridSystemVisualSingle[,,] gridSystemVisualSingleArray;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of GridSystemVisual found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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
            Transform gridSystemVisual =
                Instantiate(gridSystemVisualSinglePrefab, 
                    LevelGrid.Instance.GetWorldPos(new GridPosition(x, z, floor)), 
                    Quaternion.identity, gridSystemVisualSingleParent);

            gridSystemVisualSingleArray[x, z, floor] = gridSystemVisual.GetComponent<GridSystemVisualSingle>();
        }
    }
}