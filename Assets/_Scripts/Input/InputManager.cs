#define USE_NEW_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

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

    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Click.WasPerformedThisFrame();
#else
       return Input.GetMouseButtonDown(0);
#endif
    }

    public Vector2 GetPlayerMovementVector()
    {
#if USE_NEW_INPUT_SYSTEM
        Vector2 inputMoveDir = Vector2.zero;

        if (Keyboard.current[Key.W].wasPressedThisFrame)
            inputMoveDir += Vector2.up;

        if (Keyboard.current[Key.S].wasPressedThisFrame)
            inputMoveDir += Vector2.down;

        if (Keyboard.current[Key.A].wasPressedThisFrame)
            inputMoveDir += Vector2.left;

        if (Keyboard.current[Key.D].wasPressedThisFrame)
            inputMoveDir += Vector2.right;

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

//     public float GetCameraRotateAmount()
//     {
// #if USE_NEW_INPUT_SYSTEM
//         return playerInputActions.Player.CameraRotate.ReadValue<float>();
// #else
//         float rotateAmount = 0f;
//
//         if (Input.GetKey(KeyCode.Q))
//             rotateAmount += 1f;
//
//         if (Input.GetKey(KeyCode.E))
//             rotateAmount -= 1f;
//
//         return rotateAmount;
// #endif
//     }

//     public float GetCameraZoomAmount()
//     {
// #if USE_NEW_INPUT_SYSTEM
//         return playerInputActions.Player.CameraZoom.ReadValue<float>();
// #else
//         float zoomAmount = 0f;
//         
//         if (Input.mouseScrollDelta.y > 0)
//             zoomAmount -= 1f;
//
//         if (Input.mouseScrollDelta.y < 0)
//             zoomAmount += 1f;
//
//         return zoomAmount;
// #endif
//     }
}