using UnityEngine;

public class Box : GridElement
{
    // No special logic needed (yet)

    [SerializeField] bool isTestingBox = false;
    
    protected override void Start()
    {
        base.Start();

        if (!isTestingBox)
            return;
        
        /*
         * TODO - This data is hardcoded:
         * 1 - The extra grid positions should be added automatically
         * 2 - GetGridPositionsForDirection should be updated so it calculates what the relevant grid positions are,
         * that push the correct places depending on the direction of movment
         */
        
        // // Get X and Z scale
        // Vector3 scale = transform.localScale;
        //
        // // If scale in X is 3, the box is 3 in this axis, get the GridPosition in x-1 and x+1
        // int xScale = Mathf.RoundToInt(scale.x);
        Vector3 positionZ_Plus_1 = transform.position + new Vector3(0f, 0f, LevelGrid.Instance.GetCellSize());
        Vector3 positionZ_Minus_1 = transform.position - new Vector3(0f, 0f, LevelGrid.Instance.GetCellSize());
        
        GridPosition GridPositionZ_Plus_1 = LevelGrid.Instance.GetGridPos(positionZ_Plus_1);
        GridPosition GridPositionZ_Minus_1 = LevelGrid.Instance.GetGridPos(positionZ_Minus_1);
        
        gridPositions.Add(GridPositionZ_Plus_1);
        LevelGrid.Instance.AddGridElementAtGridPos(GridPositionZ_Plus_1, this);
        
        gridPositions.Add(GridPositionZ_Minus_1);
        LevelGrid.Instance.AddGridElementAtGridPos(GridPositionZ_Minus_1, this);
    }
}
