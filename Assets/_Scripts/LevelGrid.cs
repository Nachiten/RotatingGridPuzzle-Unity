using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public const float FLOOR_HEIGHT = 3f;
    
    public event Action OnAnyUnitMovedGridPosition;
    public static event Action OnAnyUnitChangedFloor;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private Transform gridDebugObjectParent;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private int totalFloors;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private List<GridSystem<GridObject>> gridSystems;
    
    public static LevelGrid Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
        InitializeGridSystems();
        SetWalkableGridPositions();
    }

    private void InitializeGridSystems()
    {
        gridSystems = new List<GridSystem<GridObject>>();
        
        for (int floor = 0; floor < totalFloors; floor++)
        {
            GridSystem<GridObject> gridSystem  = new(width, height, cellSize, floor, FLOOR_HEIGHT,
                (gridSystem, gridPosition) => new GridObject(gridSystem, gridPosition));
            
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab, gridDebugObjectParent);

            gridSystems.Add(gridSystem);
        }
    }
    
    private void SetWalkableGridPositions()
    {
        // By default, all grid positions are walkable
        
        for (int x = 0; x < width; x++)
        for (int z = 0; z < height; z++)
        for (int floor = 0; floor < totalFloors; floor++)
        {
            GridPosition gridPosition = new(x, z, floor);
            Vector3 worldPosition = GetWorldPos(gridPosition);
            const float raycastOffsetDistance = 1f;
            
            // Check if there is an obstacle above the grid position
            bool obstaclesRaycast = Physics.Raycast(
                worldPosition + Vector3.down * raycastOffsetDistance,
                Vector3.up,
                raycastOffsetDistance * 2,
                obstaclesLayerMask);
            
            // If there is an obstacle, set the grid position as unwalkable
            if (obstaclesRaycast)
                GetGridObjectAtGridPos(gridPosition).SetIsWalkable(false);
        }
    }

    private void InitializeSingleton()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of LevelGrid found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    private GridSystem<GridObject> GetGridSystem(int floor)
    {
        return gridSystems[floor];
    }

    public void AddUnitAtGridPos(GridPosition gridPos, Unit unit)
    {
        GetGridObjectAtGridPos(gridPos).AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPos(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).GetUnitList();
    }

    public void RemoveUnitAtGridPos(GridPosition gridPos, Unit unit)
    {
        GetGridObjectAtGridPos(gridPos).RemoveUnit(unit);
    }

    public void MoveUnitGridPos(Unit unit, GridPosition fromGridPos, GridPosition toGridPos)
    {
        RemoveUnitAtGridPos(fromGridPos, unit);
        AddUnitAtGridPos(toGridPos, unit);

        OnAnyUnitMovedGridPosition?.Invoke();
        
        // Only call this event if the unit changed floor
        if (fromGridPos.floor != toGridPos.floor)
            OnAnyUnitChangedFloor?.Invoke();
    }

    public bool GridPosIsValid(GridPosition gridPos)
    {
        return gridPos.FloorIsValid(totalFloors) && GetGridSystem(gridPos.floor).GridPosIsValid(gridPos);
    }
    
    public bool GridPosIsWalkable(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).GetIsWalkable();
    }

    public bool GridPosHasAnyUnit(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).HasAnyUnit();
    }

    public Unit GetUnitAtGridPos(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).GetUnit();
    }

    private GridObject GetGridObjectAtGridPos(GridPosition gridPos)
    {
        return GetGridSystem(gridPos.floor).GetGridObject(gridPos);
    }

    public int GetFloorFromWorldPos(Vector3 worldPos)
    {
        return Mathf.RoundToInt(worldPos.y / FLOOR_HEIGHT);
    }
    
    public GridPosition GetGridPos(Vector3 worldPos)
    {
        return GetGridSystem(GetFloorFromWorldPos(worldPos)).GetGridPos(worldPos);
    }

    public Vector3 GetWorldPos(GridPosition gridPos)
    {
        return GetGridSystem(gridPos.floor).GetWorldPos(gridPos);
    }
    
    public int GetWidth() => GetGridSystem(0).GetWidth();
    public int GetHeight() => GetGridSystem(0).GetHeight(); 
    public int GetTotalFloors() => totalFloors;
    public float GetCellSize() => cellSize;
}