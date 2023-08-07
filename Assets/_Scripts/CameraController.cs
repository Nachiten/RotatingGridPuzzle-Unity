using System;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // public static event Action OnCameraZoomed;
    
    // public static CameraController Instance { get; private set; }
    //
    // private const float MIN_FOLLOW_Y_OFFSET = 2f;
    // private const float MAX_FOLLOW_Y_OFFSET = 15f;
    //
    // [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    //
    // private CinemachineTransposer cinemachineTransposer;
    // private Vector3 targetFollowOffset;

    // private void Awake()
    // {
    //     // Instance = this;
    //     
    //     // cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    //     // targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    // }

    // private void Update()
    // {
    //     // HandleCameraMovement();
    //     // HandleCameraRotation();
    //     // HandleCameraZoom();
    // }

    // private void HandleCameraMovement()
    // {
    //     Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();
    //
    //     Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
    //
    //     const float moveSpeed = 10f;
    //     transform.position += moveVector * (moveSpeed * Time.deltaTime);
    // }
    //
    // private void HandleCameraRotation()
    // {
    //     Vector3 rotationVector = Vector3.zero;
    //
    //     rotationVector.y = InputManager.Instance.GetCameraRotateAmount();
    //
    //     const float rotationSpeed = 100f;
    //     transform.eulerAngles += rotationVector * (rotationSpeed * Time.deltaTime);
    // }
    //
    // private void HandleCameraZoom()
    // {
    //     float newOffsetY = targetFollowOffset.y + InputManager.Instance.GetCameraZoomAmount();
    //     newOffsetY = Mathf.Clamp(newOffsetY, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
    //     
    //     // This is checking if the value changed by more than 0.001f, so to not call it every frame
    //     // Rememer this code IS called every frame, but the event is not, only when the player zooms in or out
    //     const float zoomThreshold = 0.001f;
    //     if (Math.Abs(newOffsetY - targetFollowOffset.y) > zoomThreshold)
    //         OnCameraZoomed?.Invoke();
    //     
    //     targetFollowOffset.y = newOffsetY;
    //
    //     const float zoomSpeed = 10f;
    //     cinemachineTransposer.m_FollowOffset = Vector3.Slerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset,
    //         Time.deltaTime * zoomSpeed);
    // }

    // public float GetCameraHeight()
    // {
    //     return targetFollowOffset.y;
    // }
}