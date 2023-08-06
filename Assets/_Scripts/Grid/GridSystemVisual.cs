using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private Transform gridSystemVisualSingleParent;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterials;

    private GridSystemVisualSingle[,,] gridSystemVisualSingleArray;

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }

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

        UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged;
        UnitActionSystem.Instance.OnBusyChanged += OnBusyChanged;
        // LevelGrid.Instance.OnAnyUnitMovedGridPosition += OnAnyUnitMovedGridPosition;
       

        UpdateGridVisual();
    }

    private void OnBusyChanged(bool busy)
    {
        UpdateGridVisual();
    }

    // private void OnAnyUnitMovedGridPosition()
    // {
    //     UpdateGridVisual();
    // }

    private void OnSelectedActionChanged(BaseAction obj)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPositions()
    {
        foreach (GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSingleArray)
            gridSystemVisualSingle.Hide();
    }

    public void ShowGridPositions(List<GridPosition> gridPositions, GridVisualType gridVisualType)
    {
        gridPositions.ForEach(gridPosition =>
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z, gridPosition.floor]
                .Show(GetGridVisualTypeMaterial(gridVisualType));
        });
    }

    public enum GridRangeType
    {
        Square,
        Circle
    }

    private void ShowGridPositionRange(GridPosition centerGridPosition, int range, GridVisualType gridVisualType, GridRangeType rangeType = GridRangeType.Circle)
    {
        List<GridPosition> gridPositionList = new();

        for (int x = -range; x <= range; x++)
        for (int z = -range; z <= range; z++)
        {
            GridPosition possibleGridPosition = centerGridPosition + new GridPosition(x, z, 0);

            if (!LevelGrid.Instance.GridPosIsValid(possibleGridPosition))
                continue;

            int distance = centerGridPosition.Distance(possibleGridPosition);

            // Grid pos is too far away (only for not square)
            if (distance > range && rangeType == GridRangeType.Circle)
                continue;

            gridPositionList.Add(possibleGridPosition);
        }

        ShowGridPositions(gridPositionList, gridVisualType);
    }

    private void UpdateGridVisual()
    {
        HideAllGridPositions();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            default:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case GrenadeAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case InteractAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(),
                    GridVisualType.RedSoft, GridRangeType.Circle);
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(),
                    GridVisualType.RedSoft, GridRangeType.Square);
                break;
        }

        ShowGridPositions(selectedAction.GetValidGridPositionsForAction(), gridVisualType);
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        GridVisualTypeMaterial foundGridVisualType =
            gridVisualTypeMaterials.Find(x => x.gridVisualType == gridVisualType);

        if (foundGridVisualType.material)
            return foundGridVisualType.material;

        Debug.LogError("GridVisualTypeMaterial not found for GridVisualType: " + gridVisualType);
        return null;
    }
}