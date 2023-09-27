#define USE_NEW_INPUT_SYSTEM
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of InputManager found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public bool IsRestartButtonDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Restart.WasPerformedThisFrame();
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    public bool IsUndoButtonPressed()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Undo.IsPressed();
#else
        return Input.GetKeyDown(KeyCode.Z);
#endif
    }

    public bool IsRedoButtonPressed()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Redo.IsPressed();
#else 
     return Input.GetKeyDown(KeyCode.X);
#endif
    }
    
    public bool WasUndoButtonPressedThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Undo.WasPerformedThisFrame();
#else
        return Input.GetKeyDown(KeyCode.Z);
#endif
    }

    public bool WasRedoButtonPressedThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Redo.WasPerformedThisFrame();
#else 
     return Input.GetKeyDown(KeyCode.X);
#endif
    }
    
//     public Vector2 GetMouseScreenPosition()
//     {
// #if USE_NEW_INPUT_SYSTEM
//         return Mouse.current.position.ReadValue();
// #else
//         return Input.mousePosition;
// #endif
//     }

//     public bool IsMouseButtonDownThisFrame()
//     {
// #if USE_NEW_INPUT_SYSTEM
//         return playerInputActions.Player.Click.WasPerformedThisFrame();
// #else
//        return Input.GetMouseButtonDown(0);
// #endif
//     }

    private Vector2 axisPreference = Vector2.zero;
    private Vector2 prevMovingDirection = Vector2.zero;

    public Vector2 GetPlayerMovementVector()
    {
#if USE_NEW_INPUT_SYSTEM
        Vector2 inputMoveDir = playerInputActions.Player.PlayerMovement.ReadValue<Vector2>();

        // If one of the axis is 0, return value
        if (inputMoveDir.x == 0f || inputMoveDir.y == 0f)
        {
            // Set prevMovingDirection and reset axisPreference
            prevMovingDirection = inputMoveDir;
            axisPreference = Vector2.zero;
            return inputMoveDir;
        }

        // If there is axisPreference, return it
        if (axisPreference != Vector2.zero)
            return axisPreference;
        
        // There is no previous axisPreference
        
        // If it was moving on X, move on Y (set X to 0), and vice versa
        if (prevMovingDirection.x != 0)
            inputMoveDir.x = 0;
        else
            inputMoveDir.y = 0;

        axisPreference = inputMoveDir;
        return inputMoveDir;
        
#else
        Vector2 inputMoveDir = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            inputMoveDir += Vector2.up;

        if (Input.GetKey(KeyCode.S))
            inputMoveDir += Vector2.down;

        if (Input.GetKey(KeyCode.A))
            inputMoveDir += Vector2.left;

        if (Input.GetKey(KeyCode.D))
            inputMoveDir += Vector2.right;

        return inputMoveDir;
#endif
    }
}