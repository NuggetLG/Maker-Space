using UnityEngine;
using UnityEngine.ProBuilder;

public class MeshExporter : MonoBehaviour
{
    public static MeshExporter Instance { get; private set; }
    public static Mesh exportedMesh; // Variable para almacenar el Mesh exportado

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener la instancia al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Asegurar que solo haya una instancia
        }
    }

    public void ExportComposite(GameObject compositeObject)
    {
        if (compositeObject != null)
        {
            Mesh mesh = null;
            ProBuilderMesh pbMesh = compositeObject.GetComponent<ProBuilderMesh>();

            if (pbMesh != null)
            {
                pbMesh.ToMesh();
                pbMesh.Refresh();
                mesh = pbMesh.GetComponent<MeshFilter>().mesh;
            }
            else
            {
                MeshFilter meshFilter = compositeObject.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    mesh = meshFilter.mesh;
                }
                else
                {
                    Debug.LogError("No se encontró el componente MeshFilter en el objeto compuesto.");
                    return;
                }
            }

            exportedMesh = mesh; // Guardar el Mesh en lugar del objeto completo
            Debug.Log("Mesh exportado correctamente.");

            // Cambiar de escena usando LevelManager
            LevelManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("El objeto compuesto es nulo. Realice la sustracción primero.");
        }
    }
}