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

    private List<GridSystem<GridObject>> gridSystems;
    
    public static LevelGrid Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();

        InitializeGridSystems();
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
    
    private void Start()
    {
        //Pathfinding.Instance.Setup(width, height, cellSize, totalFloors);
    }

    private GridSystem<GridObject> GetGridSystem(int floor)
    {
        return gridSystems[floor];
    }

    public void AddUnitAtGridPos(GridPosition gridPos, Unit unit)
    {
        GetGridObject(gridPos).AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPos(GridPosition gridPos)
    {
        return GetGridObject(gridPos).GetUnitList();
    }

    public void RemoveUnitAtGridPos(GridPosition gridPos, Unit unit)
    {
        GetGridObject(gridPos).RemoveUnit(unit);
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

    public bool GridPosHasAnyUnit(GridPosition gridPos)
    {
        return GetGridObject(gridPos).HasAnyUnit();
    }

    public Unit GetUnitAtGridPos(GridPosition gridPos)
    {
        return GetGridObject(gridPos).GetUnit();
    }

    private GridObject GetGridObject(GridPosition gridPos)
    {
        return GetGridSystem(gridPos.floor).GetGridObject(gridPos);
    }

    public int GetFloor(Vector3 worldPos)
    {
        return Mathf.RoundToInt(worldPos.y / FLOOR_HEIGHT);
    }
    
    public GridPosition GetGridPos(Vector3 worldPos)
    {
        return GetGridSystem(GetFloor(worldPos)).GetGridPos(worldPos);
    }

    public Vector3 GetWorldPos(GridPosition gridPos)
    {
        return GetGridSystem(gridPos.floor).GetWorldPos(gridPos);
    }

    // public IInteractable GetInteractableAtGridPos(GridPosition gridPos)
    // {
    //     return GetGridObject(gridPos).GetInteractable();
    // }
    //
    // public void SetInteractableAtGridPos(GridPosition gridPos, IInteractable interactable)
    // {
    //     GetGridObject(gridPos).SetInteractable(interactable);
    // }
    //
    // public void ClearInteractableAtGridPos(GridPosition gridPosition)
    // {
    //     GetGridObject(gridPosition).ClearInteractable();
    // }

    public int GetWidth() => GetGridSystem(0).GetWidth();
    public int GetHeight() => GetGridSystem(0).GetHeight(); 
    public int GetTotalFloors() => totalFloors;
    
    public float GetCellSize() => cellSize;
}