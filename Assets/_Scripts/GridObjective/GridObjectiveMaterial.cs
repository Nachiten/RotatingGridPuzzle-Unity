using UnityEngine;

public class GridObjectiveMaterial : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private MeshRenderer quadMeshRenderer;
    
    private void Awake()
    {
        quadMeshRenderer.material = material;
    }
}
