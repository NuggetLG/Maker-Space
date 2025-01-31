using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class MeshSubtractor : MonoBehaviour
{
    public Material shapeMaterial; // Add this field to assign the material
    private GameObject composite; // Store the composite object

    public void SubtractMesh(GameObject target, GameObject hole)
    {
        // Add vertex attributes to the meshes
        MeshUtility.AddVertexAttributes(target.GetComponent<MeshFilter>().mesh);
        MeshUtility.AddVertexAttributes(hole.GetComponent<MeshFilter>().mesh);

        // Perform the boolean subtraction
        Model result = CSG.Subtract(target, hole);

        if (result == null || result.mesh == null)
        {
            Debug.LogError("Boolean operation failed or resulted in an empty mesh.");
            return;
        }

        // Create a ProBuilderMesh from the resulting mesh
        var pbMesh = ProBuilderMesh.Create(result.mesh.vertices, CreateFacesFromMesh(result.mesh));
        
        pbMesh.sharedVertices = SharedVertex.GetSharedVerticesWithPositions(pbMesh.positions);

        // Merge the faces of the ProBuilderMesh
        Face mergedFace = MergeElements.Merge(pbMesh, pbMesh.faces);

        // Ensure the mesh is updated
        pbMesh.ToMesh();
        pbMesh.Refresh();
        
        composite = pbMesh.gameObject;

        // Log the number of vertices and triangles
        Debug.Log($"Final mesh has {pbMesh.vertexCount} vertices and {pbMesh.faceCount} faces.");
    }

    private List<Face> CreateFacesFromMesh(Mesh mesh)
    {
        List<Face> faces = new List<Face>();
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            faces.Add(new Face(new int[] { mesh.triangles[i], mesh.triangles[i + 1], mesh.triangles[i + 2] }));
        }
        return faces;
    }

    public void ExportShapeFromButton()
    {
        if (composite != null)
        {
            ProBuilderMesh pbMesh = composite.GetComponent<ProBuilderMesh>();

            if (pbMesh != null)
            {
                // Ensure the mesh is updated before exporting
                pbMesh.ToMesh();
                pbMesh.Refresh();

                // Export the final mesh to an OBJ file
                MeshExporter meshExporter = new MeshExporter();
                meshExporter.ExportMeshToFbx(new Object[] { pbMesh.gameObject }, "Assets/FinalShape.fbx");

                Debug.Log("Shape exported successfully.");
            }
            else
            {
                Debug.LogError("ProBuilderMesh component not found on the composite object.");
            }
        }
        else
        {
            Debug.LogError("Composite object not found. Perform the subtraction first.");
        }
    }
}