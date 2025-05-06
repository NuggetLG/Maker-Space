using UnityEngine;

public class MeshUtility
{
    public static void AddVertexAttributes(Mesh mesh)
    {
        // Ensure the mesh has the necessary vertex attributes
        Vector3[] vertices = mesh.vertices;
        Vector3[] v1 = new Vector3[vertices.Length];
        Vector3[] v2 = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            v1[i] = vertices[i]; // Example: Copying vertices to v1
            v2[i] = vertices[i]; // Example: Copying vertices to v2
        }

        mesh.SetUVs(1, v1);
        mesh.SetUVs(2, v2);
    }
}