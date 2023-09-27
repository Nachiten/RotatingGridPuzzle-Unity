using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Box : GridElement
{
    private int xOffset;
    private int zOffset;
    
    protected override void Start()
    {
        centerGridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        
        Vector3 scale = transform.localScale;
        int xScale = Mathf.RoundToInt(scale.x);
        int zScale = Mathf.RoundToInt(scale.z);
       
        xOffset = xScale / 2;
        zOffset = zScale / 2;
        
        for (int xValue = -xOffset; xValue <= xOffset; xValue++)
        {
            for (int zValue = -zOffset; zValue <= zOffset; zValue++)
            {
                GridPosition gridPosition = centerGridPosition + new GridPosition(xValue, zValue, 0);
                gridPositions.Add(gridPosition);
                LevelGrid.Instance.AddGridElementAtGridPos(gridPosition, this);
            }
        }
        
        //LevelGrid.Instance.PrintGridPositionsList(gridPositions);
    }

    public override List<GridPosition> GetGridPositionsForDirection(GridPosition direction)
    {
        int _xOffset = direction.x switch
        {
            > 0 => xOffset,
            < 0 => -xOffset,
            _ => 0
        };

        int _zOffset = direction.z switch
        {
            > 0 => zOffset,
            < 0 => -zOffset,
            _ => 0
        };
        
        // X coord of grid pos has to be equal to the x coord of the center of the box + the offset+
        // Same with the z coord
        List<GridPosition> _gridPositions = gridPositions.Where(gridPosition =>
        {
            // Its is not moving in that direction, or it is moving in that and the grid pos obeys the condition
            bool hasXOffset = _xOffset == 0 || gridPosition.x == centerGridPosition.x + _xOffset;
            bool hasZOffset = _zOffset == 0 || gridPosition.z == centerGridPosition.z + _zOffset;
            
            return hasXOffset && hasZOffset;
        }).ToList();

        //LevelGrid.Instance.PrintGridPositionsList(_gridPositions, "Grid positions for direction");
        
        return _gridPositions;
    }
}
