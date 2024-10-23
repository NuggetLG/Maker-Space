using UnityEngine;

public class MeshExtruder : MonoBehaviour
{
    public static GameObject ExtrudeMesh(Mesh originalMesh, Vector3 center, float depth, Material material, string name)
    {
        Mesh extrudedMesh = new Mesh();

        Vector3[] originalVertices = originalMesh.vertices;
        int[] originalTriangles = originalMesh.triangles;
        Vector3[] extrudedVertices = new Vector3[originalVertices.Length * 2];
        int[] extrudedTriangles = new int[originalTriangles.Length * 2 + originalVertices.Length * 6];

        // Create vertices
        for (int i = 0; i < originalVertices.Length; i++)
        {
            extrudedVertices[i] = originalVertices[i];
            extrudedVertices[i + originalVertices.Length] = originalVertices[i] + Vector3.forward * depth;
        }

        // Create triangles
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            int a = originalTriangles[i];
            int b = originalTriangles[i + 1];
            int c = originalTriangles[i + 2];

            // Front face
            extrudedTriangles[i] = a;
            extrudedTriangles[i + 1] = b;
            extrudedTriangles[i + 2] = c;

            // Back face
            extrudedTriangles[i + originalTriangles.Length] = c + originalVertices.Length;
            extrudedTriangles[i + originalTriangles.Length + 1] = b + originalVertices.Length;
            extrudedTriangles[i + originalTriangles.Length + 2] = a + originalVertices.Length;
        }

        // Create side faces
        int index = originalTriangles.Length * 2;
        for (int i = 0; i < originalVertices.Length; i++)
        {
            int next = (i + 1) % originalVertices.Length;

            extrudedTriangles[index++] = i;
            extrudedTriangles[index++] = next;
            extrudedTriangles[index++] = i + originalVertices.Length;

            extrudedTriangles[index++] = next;
            extrudedTriangles[index++] = next + originalVertices.Length;
            extrudedTriangles[index++] = i + originalVertices.Length;
        }

        extrudedMesh.vertices = extrudedVertices;
        extrudedMesh.triangles = extrudedTriangles;
        extrudedMesh.RecalculateNormals();
        extrudedMesh.RecalculateBounds();

        // Create a new GameObject for the extruded mesh
        GameObject extrudedObject = new GameObject(name);
        MeshFilter meshFilter = extrudedObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = extrudedObject.AddComponent<MeshRenderer>();
        extrudedObject.transform.position = center;

        // Assign the extruded mesh and material
        meshFilter.mesh = extrudedMesh;
        meshRenderer.material = material;

        return extrudedObject;
    }
}