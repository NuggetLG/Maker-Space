using System.IO;
using UnityEngine;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine.ProBuilder;

public class MeshExporter : MonoBehaviour
{
    public GameObject composite; // Store the composite object

    public void ExportButton()
    {
        if (composite != null)
        {
            Mesh mesh = null;
            ProBuilderMesh pbMesh = composite.GetComponent<ProBuilderMesh>();

            if (pbMesh != null)
            {
                // Ensure the ProBuilder mesh is updated before exporting
                pbMesh.ToMesh();
                pbMesh.Refresh();
                mesh = pbMesh.GetComponent<MeshFilter>().mesh;
            }
            else
            {
                MeshFilter meshFilter = composite.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    mesh = meshFilter.mesh;
                }
                else
                {
                    Debug.LogError("MeshFilter component not found on the composite object.");
                    return;
                }
            }

            // Export the final mesh to an FBX file
            ExportMeshToFbx(new Object[] { composite }, "Assets/Exported.fbx");
            Debug.Log("Shape exported successfully.");
        }
        else
        {
            Debug.LogError("Composite object not found. Perform the subtraction first.");
        }
    }

    public void ExportMeshToFbx(Object[] gameObjects, string filePath)
    {
        ModelExporter.ExportObjects(filePath, gameObjects);
    }
}