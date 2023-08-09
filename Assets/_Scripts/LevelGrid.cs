using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public const float FLOOR_HEIGHT = 3f;

    public event Action OnAnyGridElementMovedGridPosition;
    public static event Action OnAnyGridElementChangedFloor;

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

    private void InitializeGridSystems()
    {
        gridSystems = new List<GridSystem<GridObject>>();

        for (int floor = 0; floor < totalFloors; floor++)
        {
            GridSystem<GridObject> gridSystem = new(width, height, cellSize, floor, FLOOR_HEIGHT,
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

    /// <summary>
    /// Add a GridElement to the grid at the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to add the GridElement to </param>
    /// <param name="gridElement"> The GridElement to add to the grid </param>
    public void AddGridElementAtGridPos(GridPosition gridPos, GridElement gridElement)
    {
        GetGridObjectAtGridPos(gridPos).AddGridElement(gridElement);
    }

    /// <summary>
    /// Gets the list of units at the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to get the list of units from </param>
    /// <returns> The list of units at the given grid position </returns>
    public List<GridElement> GetGridElementListAtGridPos(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).GetGridElementList();
    }

    /// <summary>
    /// Remove a GridElement from the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to remove the GridElement from </param>
    /// <param name="gridElement"> The GridElement to remove from the grid </param>
    private void RemoveGridElementAtGridPos(GridPosition gridPos, GridElement gridElement)
    {
        GetGridObjectAtGridPos(gridPos).RemoveGridElement(gridElement);
    }

    /// <summary>
    /// Move a GridElement from fromGridPos to toGridPos
    /// </summary>
    /// <param name="gridElement"> The GridElement to move </param>
    /// <param name="fromGridPos"> The origin grid position </param>
    /// <param name="toGridPos"> The destination grid position </param>
    public void MoveGridElementGridPos(GridElement gridElement, GridPosition fromGridPos, GridPosition toGridPos)
    {
        RemoveGridElementAtGridPos(fromGridPos, gridElement);
        AddGridElementAtGridPos(toGridPos, gridElement);

        OnAnyGridElementMovedGridPosition?.Invoke();

        // Only call this event if the GridElement changed floor
        if (fromGridPos.floor != toGridPos.floor)
            OnAnyGridElementChangedFloor?.Invoke();
    }

    /// <summary>
    /// Get if the given grid position is valid
    /// </summary>
    /// <param name="gridPos"> The grid position to check </param>
    /// <returns> True if the grid position is valid </returns>
    private bool GridPosIsValid(GridPosition gridPos)
    {
        return gridPos.FloorIsValid(totalFloors) && GetGridSystem(gridPos.floor).GridPosIsValid(gridPos);
    }

    /// <summary>
    /// Get if the given grid position is walkable
    /// </summary>
    /// <param name="gridPos"> The grid position to check </param>
    /// <returns> True if the grid position is walkable </returns>
    private bool GridPosIsWalkable(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).GetIsWalkable();
    }

    /// <summary>
    /// Get if the given grid position has any GridElement
    /// </summary>
    /// <param name="gridPos"> The grid position to check </param>
    /// <returns> True if the grid position has any GridElement </returns>
    private bool GridPosHasAnyGridElement(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).HasAnyGridElement();
    }

    /// <summary>
    /// Get the first GridElement at the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to get the GridElement from </param>
    /// <returns> The first GridElement at the given grid position </returns>
    private GridElement GetGridElementAtGridPos(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).GetGridElement();
    }

    /// <summary>
    /// Get the grid object at the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to get the grid object from </param>
    /// <returns> The grid object at the given grid position </returns>
    private GridObject GetGridObjectAtGridPos(GridPosition gridPos)
    {
        return GetGridSystem(gridPos.floor).GetGridObject(gridPos);
    }

    /// <summary>
    /// Get the floor from the given world position
    /// </summary>
    /// <param name="worldPos"> The world position to get the floor from </param>
    /// <returns> The floor from the given world position </returns>
    private int GetFloorFromWorldPos(Vector3 worldPos)
    {
        return Mathf.RoundToInt(worldPos.y / FLOOR_HEIGHT);
    }

    /// <summary>
    /// Get the grid position from the given world position
    /// </summary>
    /// <param name="worldPos"> The world position to get the grid position from </param>
    /// <returns> The grid position from the given world position </returns>
    public GridPosition GetGridPos(Vector3 worldPos)
    {
        return GetGridSystem(GetFloorFromWorldPos(worldPos)).GetGridPos(worldPos);
    }

    /// <summary>
    /// Get the world position from the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to get the world position from </param>
    /// <returns> The world position from the given grid position </returns>
    public Vector3 GetWorldPos(GridPosition gridPos)
    {
        return GetGridSystem(gridPos.floor).GetWorldPos(gridPos);
    }
    
    /// <summary>
    /// Move a GridElement from the given grid position in the given direction
    /// </summary>
    /// <param name="fromGridPos"> The origin grid position </param>
    /// <param name="direction"> The direction to move the GridElement in </param>
    private void MoveGridElement(GridPosition fromGridPos, GridPosition direction)
    {
        GridElement gridElementAtGridPos = GetGridElementAtGridPos(fromGridPos);

        // Cycle through all grid positions in the given direction
        gridElementAtGridPos.GetGridPositions().ForEach(_fromGridPos =>
        {
            GridPosition _toGridPos = _fromGridPos + direction;

            if (!ValidGridPosToMove(_toGridPos))
            {
                Debug.LogError("Should not happen: Invalid grid position to move to");
                return;
            }
            
            if (GridPosHasAnyGridElement(_toGridPos))
                MoveGridElement(_toGridPos, direction);

            // Get the GridElement at the origin grid pos
            GridElement gridElementAtFromGridPos = GetGridElementAtGridPos(_fromGridPos);

            // The grid element that started this call is moved after the loop finishes
            if (gridElementAtFromGridPos == gridElementAtGridPos) 
                return;
            
            // Debug log from which to which grid pos I moved
            Debug.Log($"Moved {gridElementAtFromGridPos.name} " +
                      $"from [{_fromGridPos.ToString()}] " +
                      $"to [{_toGridPos.ToString()}]");
                
            gridElementAtFromGridPos.MoveGridElementInDirection(direction);
        });

        // The grid element that started this call is moved now
        gridElementAtGridPos.MoveGridElementInDirection(direction);
    }

    /// <summary>
    /// Try to move the GridElements at the given grid positions in the given direction
    /// </summary>
    /// <param name="gridPositions"> The grid positions to move the GridElements from </param>
    /// <param name="direction"> The direction to move the GridElements in </param>
    public void TryMoveGridElements(List<GridPosition> gridPositions, GridPosition direction)
    {
        if (!CanMoveGridElements(gridPositions, direction))
        {
            Debug.Log("Can't move GridElements");
            return;
        }

        gridPositions.ForEach(gridPosition =>
        {
            GridElement gridElementAtGridPos = GetGridElementAtGridPos(gridPosition);

            gridElementAtGridPos.GetGridPositions()
                .ForEach(_gridPosition =>
                {
                    MoveGridElement(_gridPosition, direction);
                });
        });
    }
    
    /// <summary>
    /// Get if ALL the GridElements at the given grid positions can be moved in the given direction
    /// </summary>
    /// <param name="gridPositions"> The grid positions to check </param>
    /// <param name="direction"> The direction to check </param>
    /// <returns> True if ALL the GridElements at the grid positions can be moved in the direction </returns>
    private bool CanMoveGridElements(List<GridPosition> gridPositions, GridPosition direction)
    {
        return gridPositions
                .All(gridPosition => GetGridElementAtGridPos(gridPosition).GetGridPositions()
                    .All(_gridPosition => CanMoveGridElement(_gridPosition, direction)));
    }

    private bool CanMoveGridElement(GridPosition fromGridPos, GridPosition direction)
    {
        GridPosition toGridPos = fromGridPos + direction;
        
        return ValidGridPosToMove(toGridPos) && 
               (!GridPosHasAnyGridElement(toGridPos) || 
                CanMoveGridElement(toGridPos, direction));
    }

    private bool ValidGridPosToMove(GridPosition gridPos)
    {
        return GridPosIsValid(gridPos) && GridPosIsWalkable(gridPos);
    }

    private GridSystem<GridObject> GetGridSystem(int floor) => gridSystems[floor];
    public int GetWidth() => GetGridSystem(0).GetWidth();
    public int GetHeight() => GetGridSystem(0).GetHeight(); 
    public int GetTotalFloors() => totalFloors;
    public float GetCellSize() => cellSize;
}