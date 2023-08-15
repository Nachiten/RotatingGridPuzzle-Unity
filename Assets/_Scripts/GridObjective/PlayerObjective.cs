public class PlayerObjective : GridObjective
{
    protected override bool IsCorrectGridElementType(GridElement gridElement)
    {
        return gridElement.GetComponent<Player>() != null;
    }
}
