using System;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private readonly float cellSize;

    private readonly TGridObject[,] gridObjects;
    private readonly int height;
    private readonly int width;
    private readonly int floor;
    private readonly float floorHeight;

    public GridSystem(int width, int height, float cellSize, int floor, float floorHeight,
        Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floor = floor;
        this.floorHeight = floorHeight;
        
        gridObjects = new TGridObject[width, height];
        
        for (int x = 0; x < width; x++)
        for (int z = 0; z < height; z++)
            gridObjects[x, z] = createGridObject(this, new GridPosition(x, z, floor));
    }

    public Vector3 GetWorldPos(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize + 
               new Vector3(0, gridPosition.floor * floorHeight, 0);
    }

    public GridPosition GetGridPos(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int z = Mathf.RoundToInt(worldPosition.z / cellSize);

        return new GridPosition(x, z, floor);
    }

    public void CreateDebugObjects(Transform debugPrefab, Transform debugPrefabParent)
    {
        for (int x = 0; x < width; x++)
        for (int z = 0; z < height; z++)
        {
            GridPosition gridPosition = new(x, z, floor);
    
            const float heightOffset = 2.1f;
            Transform debugTransform = GameObject.Instantiate(
                debugPrefab,
                GetWorldPos(gridPosition) + Vector3.up * heightOffset, 
                Quaternion.identity, 
                debugPrefabParent);
    
            GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
            gridDebugObject.SetGridObject(GetGridObject(gridPosition));
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjects[gridPosition.x, gridPosition.z];
    }

    public bool GridPosIsValid(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&
               gridPosition.x < width &&
               gridPosition.z >= 0 &&
               gridPosition.z < height &&
               gridPosition.floor == floor;
    }

    public int GetWidth() => width;
    public int GetHeight() => height;
}