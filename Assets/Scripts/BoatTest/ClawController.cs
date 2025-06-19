using System.Collections;
using UnityEngine;

public class ClawController : MonoBehaviour
{
    public GameObject[] objectPrefabs; // Lista de prefabs de objetos
    public Transform clawPosition; // Posición de la pinza
    public float moveSpeed; // Velocidad de movimiento
    public KeyCode dropKey = KeyCode.Space; // Tecla para soltar el objeto
    public float cooldownTime; // Tiempo de cooldown en segundos

    private GameObject currentObject; // Objeto actual en la pinza
    private int currentIndex = 0; // Índice del objeto actual
    private bool isObjectAttached = false; // Indica si el objeto se acopló exitosamente

    [SerializeField]
    private float offsetAboveCenterOfMass; // Distancia fija por encima del centro de masa

    [SerializeField]
    private float minHeight;
    [SerializeField]
    private GameManager gameManager;

    private void Start()
    {
        SpawnNextObject(); // Generar el primer objeto
    }

    private void Update()
    {
        // Movimiento horizontal de la pinza
        float move = Input.GetAxis("Horizontal"); // Usa las teclas A y D
        transform.Translate(Vector3.right * move * moveSpeed * Time.deltaTime);

        // Ajustar la posición vertical usando un Raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Boat") || hit.collider.gameObject.CompareTag("Carga")) // Verificar si el objeto tiene el tag "Boat"
            {
                float targetHeight = Mathf.Max(hit.point.y + offsetAboveCenterOfMass, minHeight);
                Vector3 targetPosition = new Vector3(
                    transform.position.x,
                    targetHeight,
                    transform.position.z
                );
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            }
        }

        // Soltar el objeto
        if (Input.GetKeyDown(dropKey) && currentObject != null)
        {
            DropObject();
        }
    }

    private void SpawnNextObject()
    {
        if (currentObject != null) return; // Asegurarse de que no haya un objeto activo o acoplado

        // Instanciar el nuevo objeto
        currentObject = Instantiate(objectPrefabs[currentIndex], clawPosition.position, Quaternion.identity);
        
        currentObject.transform.SetParent(clawPosition);
        currentObject.transform.localRotation = Quaternion.Euler(0, 180, 0);


        // Configurar el Rigidbody
        Rigidbody rb = currentObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Desactivar gravedad inicialmente
            rb.isKinematic = true; // Desactivar físicas
        }

        Debug.Log(currentObject.name + " ha sido generado.");
    }

    private void DropObject()
    {
        // Soltar el objeto
        currentObject.transform.SetParent(null); // Desvincular de la pinza
        Rigidbody rb = currentObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true; // Activar gravedad
            rb.isKinematic = false; // Activar físicas
        }
    }

    public void OnObjectAttached()
    { 
        Debug.Log("fue 1");
        // Verificar si se llegó al último objeto
        if (currentIndex == objectPrefabs.Length - 1)
        {
            gameManager.Conteo();
            return; // No generar más objetos
        }

        // Seleccionar el siguiente prefab
        currentIndex = (currentIndex + 1) % objectPrefabs.Length;
        Debug.Log("fue 3");

        isObjectAttached = true;
        currentObject = null; // Limpiar referencia al objeto actual
        SpawnNextObject(); // Generar el siguiente objeto
    }
    
    public void OnObjectfailed()
    {
        // Llamado por AttachBoat cuando el objeto se acople exitosamente
        isObjectAttached = false;
        currentObject = null; // Limpiar referencia al objeto actual
        SpawnNextObject(); // Generar el siguiente objeto
    }
}