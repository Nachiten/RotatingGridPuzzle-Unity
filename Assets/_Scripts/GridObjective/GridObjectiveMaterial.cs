using UnityEngine;

public class GridObjectiveMaterial : MonoBehaviour
{
    [SerializeField] private ObjectiveMaterialSO objectiveMaterialSO;
    [SerializeField] private MeshRenderer quadMeshRenderer;

    public void SetMaterial(ObjectiveType objectiveType)
    {
        quadMeshRenderer.material = objectiveMaterialSO.GetMaterialByObjectiveType(objectiveType);
    }
}
