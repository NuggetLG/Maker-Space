using System;
using UnityEngine;

public class FinalShape : MonoBehaviour
{
    [SerializeField] private GameObject firstShape;
    [SerializeField] private GameObject secondShape;
    [SerializeField] private Material resultMaterial;
    [SerializeField] private CameraController cameraController;

    public void GenerateMeshForAnimation(Mesh mesh, Vector3 center, string name, Material material, int sortingOrder = 0)
    {
        if (mesh == null)
        {
            Debug.LogError("Generated mesh is null.");
            return;
        }

        GameObject filledShape = new GameObject(name);
        MeshFilter meshFilter = filledShape.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = filledShape.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.material = material;
        meshRenderer.sortingOrder = sortingOrder; // Set the sorting order
        filledShape.transform.position = center;

        if (name == "FirstShape")
        {
            firstShape = filledShape;
            //cameraController.AdjustCameraToFitShape(firstShape);
        }
        if (name == "HoleShape")
        {
            secondShape = filledShape;
        }
        
    }

    public Mesh ConvertSpriteToMesh(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogError("Sprite is null.");
            return null;
        }

        Mesh mesh = new Mesh
        {
            vertices = Array.ConvertAll(sprite.vertices, v => (Vector3)v),
            triangles = Array.ConvertAll(sprite.triangles, i => (int)i),
            uv = sprite.uv
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
    
    
}