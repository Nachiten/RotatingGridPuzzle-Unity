public class BoxObjective : GridObjective
{
    protected override ObjectiveType GetObjectiveType()
    {
        return ObjectiveType.Box;
    }
    
    protected override bool IsCorrectGridElementType(GridElement gridElement)
    {
        return gridElement.GetComponent<Box>() != null;
    }
}
