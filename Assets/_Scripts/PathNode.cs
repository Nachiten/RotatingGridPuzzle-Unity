public class PathNode
{
    private GridPosition gridPosition;

    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode cameFromPathNode;

    private bool isWalkable = true;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return gridPosition.ToString();
    }

    public int GetGCost()
    {
        return gCost;
    }

    public int GetHCost()
    {
        return hCost;
    }

    public int GetFCost()
    {
        return fCost;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }

    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
    }

    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public void ResetCameFromPathNode()
    {
        cameFromPathNode = null;
    }

    public void Setup()
    {
        SetGCost(int.MaxValue);
        SetHCost(0);
        CalculateFCost();
        ResetCameFromPathNode();
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public void SetCameFromPathNode(PathNode cameFromPathNode)
    {
        this.cameFromPathNode = cameFromPathNode;
    }

    public PathNode GetCameFromPathNode()
    {
        return cameFromPathNode;
    }
}