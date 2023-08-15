using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    private Dictionary<GridPosition, bool> completedGridObjectives;

    private void Awake()
    {
        completedGridObjectives = new Dictionary<GridPosition, bool>();
        
        GridObjective.OnAnyGridObjectiveSpawned += OnAnyGridObjectiveSpawned;
    }

    private void Start()
    {
        GridObjective.OnAnyGridObjectiveCompleted += OnAnyGridObjectiveCompleted;
        GridObjective.OnAnyGridObjectiveUncompleted += OnAnyGridObjectiveUncompleted;
    }

    private void OnAnyGridObjectiveSpawned(GridPosition gridPosition)
    {
        completedGridObjectives[gridPosition] = false;
    }

    private void OnAnyGridObjectiveUncompleted(GridPosition gridPosition)
    {
        completedGridObjectives[gridPosition] = false;
        // PrintCompletedGridObjects();
    }

    private void OnAnyGridObjectiveCompleted(GridPosition gridPosition)
    {
        completedGridObjectives[gridPosition] = true;
        // PrintCompletedGridObjects();

        CheckWiningState();
    }
    
    private void CheckWiningState()
    {
        // All grid objects must be completed
        if (!completedGridObjectives.All(x => x.Value))
            return;

        Debug.Log("You win!");
    }
    
    private void PrintCompletedGridObjects()
    {
        foreach (var completedGridObject in completedGridObjectives)
        {
            Debug.Log($"GridPosition: {completedGridObject.Key}, Completed: {completedGridObject.Value}");
        }
    }
}
