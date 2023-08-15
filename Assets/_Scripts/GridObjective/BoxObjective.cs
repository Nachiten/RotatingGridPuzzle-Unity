public class BoxObjective : GridObjective
{
    protected override bool IsCorrectGridElementType(GridElement gridElement)
    {
        return gridElement.GetComponent<Box>() != null;
    }
}
