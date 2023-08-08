using System.Collections.Generic;

public class GridObject
{
    private GridPosition gridPosition;
    private GridSystem<GridObject> gridSystem;
    private readonly List<Unit> unitList;
    private bool isWalkable = true;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Unit>();
    }

    public override string ToString()
    {
        string unitString = "";

        foreach (Unit unit in unitList)
            unitString += unit.name + "\n";

        return $"{gridPosition.ToString()}\n{unitString}";
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        return HasAnyUnit() ? unitList[0] : null;
    }
    
    public void SetIsWalkable(bool _isWalkable) => isWalkable = _isWalkable;
    public bool GetIsWalkable() => isWalkable;
}