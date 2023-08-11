using System;
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
        int _xOffet = direction.x switch
        {
            > 0 => xOffset,
            < 0 => -xOffset,
            _ => 0
        };

        int _zOffet = direction.z switch
        {
            > 0 => zOffset,
            < 0 => -zOffset,
            _ => 0
        };

        // La coord x de la grid pos tiene q ser = a la coord x del centro de la box + el offset
        // Idem la coord z
        List<GridPosition> _gridPositions = gridPositions.Where(gridPosition =>
        {
            // Its is not moving in that direction, or it is moving in that and the grid pos obeys the condition
            bool hasXOffset = _xOffet == 0 || gridPosition.x == centerGridPosition.x + _xOffet;
            bool hasZOffset = _zOffet == 0 || gridPosition.z == centerGridPosition.z + _zOffet;
            
            return hasXOffset && hasZOffset;
        }).ToList();

        LevelGrid.Instance.PrintGridPositionsList(_gridPositions, "Grid positions for direction");
        
        return _gridPositions;
    }
}
