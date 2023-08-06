public class EnemyAIAction
{
    public GridPosition gridPosition;
    public int actionValue;

    public EnemyAIAction(GridPosition gridPosition, int actionValue)
    {
        this.gridPosition = gridPosition;
        this.actionValue = actionValue;
    }
}