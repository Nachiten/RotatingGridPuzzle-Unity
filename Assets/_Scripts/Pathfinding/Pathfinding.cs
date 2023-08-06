using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    // [SerializeField] private Transform gridDebugObjectPrefab;
    // [SerializeField] private Transform gridDebugObjectParent;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private Transform pathfindingLinkContainer;

    private List<GridSystem<PathNode>> gridSystems;
    private List<PathfindingLink> pathfindingLinks;

    private int width;
    private int height;
    private float cellSize;
    private int totalFloors;

    private void InitializeSingleton()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of Pathfinding found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Awake()
    {
        InitializeSingleton();
    }

    public void Setup(int width, int height, float cellSize, int totalFloors)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.totalFloors = totalFloors;

        InitializeGridSystems();

        SetWalkableNodes();
        
        InitializePathfindingLinks();
    }

    private void InitializePathfindingLinks()
    {
        pathfindingLinks = new List<PathfindingLink>();

        foreach (Transform pathfindingLinkTransform in pathfindingLinkContainer)
        {
            PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour = pathfindingLinkTransform.GetComponent<PathfindingLinkMonoBehaviour>();
            pathfindingLinks.Add(pathfindingLinkMonoBehaviour.GetPathfindingLink());
        }
    }

    private void SetWalkableNodes()
    {
        for (int x = 0; x < width; x++)
        for (int z = 0; z < height; z++)
        for (int floor = 0; floor < totalFloors; floor++)
        {
            GridPosition gridPosition = new(x, z, floor);
            Vector3 worldPosition = LevelGrid.Instance.GetWorldPos(gridPosition);
            const float raycastOffsetDistance = 1f;
            
            GetNode(x, z, floor).SetIsWalkable(false);
            
            bool floorRaycast = Physics.Raycast(
                worldPosition + Vector3.up * raycastOffsetDistance,
                Vector3.down,
                raycastOffsetDistance * 2,
                floorLayerMask);
            
            bool obstaclesRaycast = Physics.Raycast(
                worldPosition + Vector3.down * raycastOffsetDistance,
                Vector3.up,
                raycastOffsetDistance * 2,
                obstaclesLayerMask);

            if (floorRaycast && !obstaclesRaycast)
                GetNode(x, z, floor).SetIsWalkable(true);
        }
    }

    private void InitializeGridSystems()
    {
        gridSystems = new List<GridSystem<PathNode>>();
        
        for(int floor = 0; floor < totalFloors; floor++)
            gridSystems.Add(new GridSystem<PathNode>(width, height, cellSize, floor, 
                LevelGrid.FLOOR_HEIGHT, (_, gp) => new PathNode(gp)));
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new();
        HashSet<PathNode> closedList = new();

        PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
        PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < width; x++)
        for (int z = 0; z < height; z++)
        for (int floor = 0; floor < totalFloors; floor++)
        {
            GridPosition gridPosition = new(x, z, floor);
            PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition);

            pathNode.Setup();
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                // Reached final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbourList(currentNode).Where(neighbourNode => !closedList.Contains(neighbourNode)))
            {
                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost =
                    currentNode.GetGCost() +
                    CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tentativeGCost >= neighbourNode.GetGCost()) 
                    continue;
                
                neighbourNode.SetCameFromPathNode(currentNode);
                neighbourNode.SetGCost(tentativeGCost);
                neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                neighbourNode.CalculateFCost();

                if (!openList.Contains(neighbourNode)) 
                    openList.Add(neighbourNode);
            }
        }

        // No path found
        pathLength = 0;
        return null;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodes = new() { endNode };

        PathNode currentNode = endNode;

        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodes.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodes.Reverse();

        // Make a new list, and map all path nodes to their grid positions
        return pathNodes.Select(pathNode => pathNode.GetGridPosition()).ToList();
    }

    private int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        
        int diagonalDistance = Mathf.Min(xDistance, zDistance);
        int straightDistance = Mathf.Abs(xDistance - zDistance);
        
        return MOVE_DIAGONAL_COST * diagonalDistance + MOVE_STRAIGHT_COST * straightDistance;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];

        for (int i = 1; i < pathNodeList.Count; i++)
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
                lowestFCostPathNode = pathNodeList[i];

        return lowestFCostPathNode;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0, gridPosition.floor));
            
            if (gridPosition.z - 1 >= 0)
                // Left Down
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor));

            if (gridPosition.z + 1 < height)
                // Left Up
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor));
        }

        if (gridPosition.x + 1 < width)
        {
            // Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0, gridPosition.floor));
            
            if (gridPosition.z - 1 >= 0)
                // Right Down
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor));
            if (gridPosition.z + 1 < height)
                // Right Up
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor));
        }

        if (gridPosition.z - 1 >= 0)
            // Down
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1, gridPosition.floor));
        
        if (gridPosition.z + 1 < height)
            // Up
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1, gridPosition.floor));
        
        List<GridPosition> pathfindingLinkConnectedGridPositions = GetPathfindingLinkConecctedGridPositions(gridPosition);
        neighbourList.AddRange(pathfindingLinkConnectedGridPositions.Select(gridPos => GetNode(gridPos.x, gridPos.z, gridPos.floor)));
        
        return neighbourList;
    }

    private List<GridPosition> GetPathfindingLinkConecctedGridPositions(GridPosition gridPosition)
    {
        List<GridPosition> gridPositions = new();

        foreach (PathfindingLink pathfindingLink in pathfindingLinks)
        {
            if (pathfindingLink.gridPositionA == gridPosition)
                gridPositions.Add(pathfindingLink.gridPositionB);
            else if (pathfindingLink.gridPositionB == gridPosition)
                gridPositions.Add(pathfindingLink.gridPositionA);
        }
        
        return gridPositions;
    }

    private GridSystem<PathNode> GetGridSystem(int floor)
    {
        return gridSystems[floor];
    }

    private PathNode GetNode(int x, int z, int floor)
    {
        return GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
    }

    public bool GridPosIsWalkable(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
    }

    public void SetGridPosIsWalkable(GridPosition gridPosition, bool isWalkable)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int _) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}