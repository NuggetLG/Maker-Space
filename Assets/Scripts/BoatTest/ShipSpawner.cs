using UnityEngine;
using TMPro;
using System.Collections;

public class ShipSpawner : MonoBehaviour
{
    public Vector3 spawnPosition = Vector3.zero; // Posición de generación del barco
    public Quaternion spawnRotation = Quaternion.identity; // Rotación de generación del barco
    public float countdownTime = 3f; // Tiempo de cuenta regresiva
    [SerializeField] private TextMeshProUGUI countdownText; // Referencia al TextMeshPro
    [SerializeField] private GameObject canvas;
    [SerializeField] private float pesomaterial = 1.25f; // Peso del material del barco
    [SerializeField] private Material shipMaterial; // Variable para el material del barco
    [SerializeField] private GameObject existingShip; // Cambia el nombre según sea necesario


    private void Awake()
    {
        StartCoroutine(CountdownAndSpawn());
    }

    private IEnumerator CountdownAndSpawn()
    {
        // Cuenta regresiva
        float timeLeft = countdownTime;
        while (timeLeft > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(timeLeft).ToString(); // Actualizar el texto
            }
            Debug.Log($"Cuenta regresiva: {Mathf.Ceil(timeLeft)}"); // Mostrar en consola
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        
        if (countdownText != null)
        {
            countdownText.text = "GO";
        }
        yield return new WaitForSeconds(1f);
        
        // Generar el barco
        SpawnShip();

        // Limpiar el texto después de la cuenta regresiva
        if (countdownText != null)
        {
            canvas.SetActive(false);
        }
    }

    private void SpawnShip()
{
    if (existingShip != null && MeshExporter.exportedMesh != null)
    {
        // Configurar la posición inicial en (0, 0, 0) y rotación
        existingShip.transform.position = new Vector3(0, 10, 0);
        existingShip.transform.rotation = Quaternion.identity;

        // Asegurar que el Rigidbody esté configurado como no kinematic
        Rigidbody rb = existingShip.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = existingShip.AddComponent<Rigidbody>();
        }
        rb.isKinematic = false; // Desactivar kinematic
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        // Asignar el tag "Boat"
        existingShip.tag = "Boat";

        // Añadir o actualizar el MeshFilter con el Mesh exportado
        MeshFilter meshFilter = existingShip.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = existingShip.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = MeshExporter.exportedMesh;

        // Añadir o actualizar el MeshRenderer con el material
        MeshRenderer meshRenderer = existingShip.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = existingShip.AddComponent<MeshRenderer>();
        }
        meshRenderer.material = shipMaterial != null ? shipMaterial : new Material(Shader.Find("Standard"));
        
        // Añadir o actualizar el MeshCollider y hacerlo convexo
        MeshCollider meshCollider = existingShip.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = existingShip.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = MeshExporter.exportedMesh; // Asignar el Mesh exportado
        meshCollider.convex = true;

        // Añadir o actualizar el script Boat2d y asignar valores
        Boat2d boat2d = existingShip.GetComponent<Boat2d>();
        if (boat2d == null)
        {
            boat2d = existingShip.AddComponent<Boat2d>();
        }
        boat2d.area = GridVectorManager.areaInicial;
        boat2d.weight = GridVectorManager.areatotal * 0.2f * pesomaterial;

        Debug.Log("Barco existente modificado con el Mesh exportado.");
    }
    else if (existingShip == null)
    {
        Debug.LogError("No se encontró un barco existente para modificar.");
    }
    else
    {
        Debug.LogError("No se encontró un Mesh exportado para modificar el barco.");
    }
}
}