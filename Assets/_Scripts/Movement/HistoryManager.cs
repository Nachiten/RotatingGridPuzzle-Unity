using System;
using System.Collections.Generic;
using UnityEngine;

enum MovementActionPressed
{
    Undo,
    Redo,
    None
}

public class HistoryManager : MonoBehaviour
{
    public static HistoryManager Instance { get; private set; }

    private List<MovementHistory> movementHistories;
    private int historyIndex = -1;
    private MovementActionPressed movementActionPressed = MovementActionPressed.None;
    
    private float historyRepeatTimer;
    private const float historyRepeatTimerMax = 0.25f;
    
    private void Awake()
    {
        Instance = this;
        
        movementHistories = new List<MovementHistory>();
        historyRepeatTimer = historyRepeatTimerMax;
    }
    
    private void Update()
    {
        if (InputManager.Instance.WasUndoButtonPressedThisFrame())
        {
            UndoMovement();
            movementActionPressed = MovementActionPressed.Undo;
        }
        else if (InputManager.Instance.WasRedoButtonPressedThisFrame())
        {
            RedoMovement();
            movementActionPressed = MovementActionPressed.Redo;
        }
        else if (!InputManager.Instance.IsUndoButtonPressed() && !InputManager.Instance.IsRedoButtonPressed())
        {
            movementActionPressed = MovementActionPressed.None;
            ResetTimer();
        }
        
        if (!ActionIsNone())
            HandleHistoryRepeatTimer();
    }
    
    private bool ActionIsNone()
    {
        return movementActionPressed == MovementActionPressed.None;
    }

    private void ResetTimer()
    {
        historyRepeatTimer = historyRepeatTimerMax;
    }

    private void HandleHistoryRepeatTimer()
    {
        historyRepeatTimer -= Time.deltaTime;
        
        if (historyRepeatTimer > 0f)
            return;
        
        // If timer is finished, reset timer and perform the action pressed
        ResetTimer();;
        
        switch (movementActionPressed)
        {
            case MovementActionPressed.Undo:
                UndoMovement();
                break;
            case MovementActionPressed.Redo:
                RedoMovement();
                break;
            case MovementActionPressed.None:
                Debug.LogError("Timer should not run if no movement action is pressed");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UndoMovement()
    {
        // If there is no history to undo
        if (historyIndex == -1)
        {
            Debug.Log("Nothing to undo");
            return;
        }
        
        MovementHistory prevHistory = movementHistories[historyIndex--];
        
        // Invert the movement direction
        GridPosition invertedDirection = prevHistory.direction * new GridPosition(-1, -1, 0);
        
        // Move the grid elements back in time
        prevHistory.gridElements.ForEach(gridElement =>
        {
            gridElement.MoveGridElementInDirection(invertedDirection);
        });
    }
    
    private void RedoMovement()
    {
        // If there is no history to redo
        if (historyIndex == movementHistories.Count - 1)
        {
            Debug.Log("Nothing to redo");
            return;
        }
        
        MovementHistory nextHistory = movementHistories[++historyIndex];
        
        // Move the grid elements forward in time
        nextHistory.gridElements.ForEach(gridElement =>
        {
            gridElement.MoveGridElementInDirection(nextHistory.direction);
        });
    }

    public void AddHistory(List<GridElement> gridElements, GridPosition direction)
    {
        // If the history index is not at the end of the list, remove all elements after it
        if (historyIndex != movementHistories.Count - 1)
            movementHistories.RemoveRange(historyIndex + 1, movementHistories.Count - historyIndex - 1);
        
        movementHistories.Add(new MovementHistory(gridElements, direction));
        historyIndex++;
    }
}
