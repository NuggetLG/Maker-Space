using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Parabox.CSG;

public class MeshSubtractor : MonoBehaviour
{
    public Material shapeMaterial; // Add this field to assign the material

    public void SubtractMesh(GameObject target, GameObject hole)
    {
        // Perform the boolean subtraction
        Model result = CSG.Subtract(target, hole);

        var composite = new GameObject("FinalShape");
        var meshFilter = composite.AddComponent<MeshFilter>();
        var meshRenderer = composite.AddComponent<MeshRenderer>();

        meshFilter.sharedMesh = result.mesh;
        meshRenderer.sharedMaterial = shapeMaterial; // Use the same material for the whole shape
    }
}