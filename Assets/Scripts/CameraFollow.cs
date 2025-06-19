using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Magnet; // Primer objeto a seguir
    private Transform target; // Primer objeto a seguir
    public Vector3 offset; // Desplazamiento de la cámara
    public float smoothSpeed = 0.125f; // Velocidad de suavizado
    public float minZoom = 40f; // Zoom mínimo
    public float maxZoom = 10f; // Zoom máximo
    public float zoomLimiter = 50f; // Limitador de zoom
    public bool focusOnTarget;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (target == null)
        {
            GameObject Ship = GameObject.FindWithTag("Boat");
            if (Ship != null)
            {
                target = Ship.transform;
            }
        }
        if (focusOnTarget && target != null)
        {
            Debug.Log("Enfocando solo al target");
            // Enfocar directamente en el target
            transform.position = target.position + offset;

            // Ajustar el zoom de la cámara basado únicamente en el target
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, minZoom, smoothSpeed);
        }
        else
        {
            // Calcular el punto medio entre los dos objetos
            Vector3 centerPoint = (target.position + Magnet.position) / 2;

            // Ajustar la posición de la cámara
            Vector3 newPosition = centerPoint + offset;
            transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed);

            // Ajustar el zoom de la cámara
            float distance = Vector3.Distance(target.position, Magnet.position);
            float newZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, smoothSpeed);
        }
    }
}