using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float pointPlacementRadius = 10f;
    public Vector3 firstVectorPoint;

    public Vector2 minCameraPosition;
    public Vector2 maxCameraPosition;

    private Camera cam;
    private bool isShapeCentered = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!isShapeCentered)
        {
            HandleMovement();
            HandleZoom();
        }
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 newPositionX = transform.position + new Vector3(moveX, 0, 0);
        Vector3 newPositionY = transform.position + new Vector3(0, moveY, 0);

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = minCameraPosition.x + camWidth;
        float maxX = maxCameraPosition.x - camWidth;
        float minY = minCameraPosition.y + camHeight;
        float maxY = maxCameraPosition.y - camHeight;

        if (newPositionX.x >= minX && newPositionX.x <= maxX)
        {
            transform.position = new Vector3(newPositionX.x, transform.position.y, transform.position.z);
        }

        if (newPositionY.y >= minY && newPositionY.y <= maxY)
        {
            transform.position = new Vector3(transform.position.x, newPositionY.y, transform.position.z);
        }
        else
        {
            Debug.Log("Cannot move beyond camera limits");
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);

            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            float minX = minCameraPosition.x + camWidth;
            float maxX = maxCameraPosition.x - camWidth;
            float minY = minCameraPosition.y + camHeight;
            float maxY = maxCameraPosition.y - camHeight;

            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = newPosition;
        }
    }

    public void AdjustCameraToFitShape(GameObject shape)
    {
        Renderer renderer = shape.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Shape does not have a Renderer component.");
            return;
        }

        Bounds bounds = renderer.bounds;
        float camHeight = bounds.size.y;
        float camWidth = camHeight * cam.aspect;
        cam.orthographicSize = Mathf.Max(camHeight, camWidth) / 2f;
        cam.transform.position = new Vector3(bounds.center.x, bounds.center.y, cam.transform.position.z);

        isShapeCentered = true;
    }
    
    public bool IsWithinPlacementRadius(Vector3 point)
    {
        if (firstVectorPoint == Vector3.zero)
            return true;

        float distance = Vector3.Distance(firstVectorPoint, point);
        return distance <= pointPlacementRadius;
    }

    public void EnableMovement()
    {
        isShapeCentered = false;
    }
}