using System;
using System.Collections.Generic;

[Serializable]
public struct MovementHistory
{
    public List<GridElement> gridElements;
    public GridPosition direction;

    public MovementHistory(List<GridElement> _gridElements, GridPosition _direction)
    {
        gridElements = _gridElements;
        direction = _direction;
    }
}
