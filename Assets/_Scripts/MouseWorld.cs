using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld Instance;

    [SerializeField] private LayerMask mousePlaneLayerMask;

    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        transform.position = GetPositionOnlyHitVisible();
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Instance.mainCamera.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance.mousePlaneLayerMask);

        return raycastHit.point;
    }

    public static Vector3 GetPositionOnlyHitVisible()
    {
        Ray ray = Instance.mainCamera.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        RaycastHit[] raycastHits = Physics.RaycastAll(ray, float.MaxValue, Instance.mousePlaneLayerMask);

        System.Array.Sort(raycastHits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (RaycastHit raycastHit in raycastHits)
        {
            // If there is a renderer and it is enabled, return the point
            if (raycastHit.transform.TryGetComponent(out Renderer renderer) && renderer.enabled)
                return raycastHit.point;
        }
        
        return Vector3.zero;
    }
}