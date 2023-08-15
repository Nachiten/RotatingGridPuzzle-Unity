public class PlayerObjective : GridObjective
{
    protected override ObjectiveType GetObjectiveType()
    {
        return ObjectiveType.Player;
    }
    
    protected override bool IsCorrectGridElementType(GridElement gridElement)
    {
        return gridElement.GetComponent<Player>() != null;
    }
}
