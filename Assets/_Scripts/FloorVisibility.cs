using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    [SerializeField] private bool dymanicFloorPosition;
    [SerializeField] private List<Renderer> ignoredRenderers;
    
    private Renderer[] renderers;
    private int floor;
    
    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        floor = LevelGrid.Instance.GetFloor(transform.position);

        if (floor == 0 && !dymanicFloorPosition)
        {
            // Ground floor does not need to be hidden
            Destroy(this);
            return;
        }
        
        // Always update floor visibility on cameza zoom
        CameraController.OnCameraZoomed += OnCameraZoomed;
        // Only update floor if it is dynamic
        if (dymanicFloorPosition)
            LevelGrid.OnAnyUnitChangedFloor += OnAnyUnitChangedFloor;
    }

    private void OnDestroy()
    {
        CameraController.OnCameraZoomed -= OnCameraZoomed;
        LevelGrid.OnAnyUnitChangedFloor -= OnAnyUnitChangedFloor;
    }

    private void OnAnyUnitChangedFloor()
    {
        UpdateUnitFloor();
        UpdateFloorVisiblity();
    }

    private void OnCameraZoomed()
    {
        UpdateFloorVisiblity();
    }
    
    private void UpdateUnitFloor()
    { 
        floor = LevelGrid.Instance.GetFloor(transform.position);
    }

    private void UpdateFloorVisiblity()
    {
        float cameraHeight = CameraController.Instance.GetCameraHeight();
        const float floorHeightOffset = 6.5f;
        
        bool showObject = cameraHeight > LevelGrid.FLOOR_HEIGHT * floor + floorHeightOffset;
        
        SetShow(showObject || floor == 0);
    }

    private void SetShow(bool show)
    {
        foreach (Renderer _renderer in renderers)
        {
            if (ignoredRenderers.Contains(_renderer))
                continue;

            _renderer.enabled = show;
        }
    }
}
