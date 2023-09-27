using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ObjectiveMaterial
{
    public ObjectiveType objectiveType;
    public Material material;
}

[CreateAssetMenu(menuName = "ScriptableObjects/ObjectiveMaterialSO")]
public class ObjectiveMaterialSO : ScriptableObject
{
    // public ObjectiveMaterial[] objectiveMaterials;
    
    // public Material GetMaterialByObjectiveType(ObjectiveType objectiveType)
    // {
    //     return (
    //         from objectiveMaterial in objectiveMaterials 
    //         where objectiveMaterial.objectiveType == objectiveType 
    //         select objectiveMaterial.material)
    //         .FirstOrDefault();
    // }
}

