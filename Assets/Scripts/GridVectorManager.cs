using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class GridVectorManager : MonoBehaviour
{
    [SerializeField] private Tilemap drawingTilemap;
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private Tile vectorPointTile;
    [SerializeField] private Tile hoverTile;
    [SerializeField] private Tile hoverOutofBonds;
    [SerializeField] private Tile vectorPointTileHole;
    [SerializeField] private Tile hoverTileHole;
    [SerializeField] private Tile hoverOutofBondsHole;
    [SerializeField] private LineRenderer lineRendererPrefab;
    [SerializeField] private LineRenderer holeLineRendererPrefab;
    [SerializeField] private float pixelsPerUnit = 100f;
    [SerializeField] private FinalShape finalShape;
    [SerializeField] private GameObject Ui;
    [SerializeField] private GameObject Ui2;
    [SerializeField] private SpriteGenerationProgress progressTracker;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Material shapeMaterial;
    [SerializeField] private Material holeMaterial;
    [SerializeField] private MeshSubtractor meshSubtractor;
    public static float areaInicial;
    [SerializeField] private float areaHueco;
    public static float areatotal; // Variable para almacenar el área total
    private GameObject extrudedObject;
    private GameObject extrudedHole;
    private Grid grid;
    private List<Vector3Int> vectorPointsList = new List<Vector3Int>();
    private LineRenderer currentLine;
    private LineRenderer currentHoleLine; // New LineRenderer instance for hole mode
    private Vector3Int previousMousePos = new Vector3Int();
    private bool message = false;
    private bool holeMode = false;
    private ProBuilderMesh pbMeshObj;
    private ProBuilderMesh pbMeshHole;

    private void Start()
    {
        extrudedObject = new GameObject("ExtrudedObject");
        pbMeshObj = extrudedObject.AddComponent<ProBuilderMesh>();

        extrudedHole = new GameObject("ExtrudedHole");
        pbMeshHole = extrudedHole.AddComponent<ProBuilderMesh>();

        grid = GetComponent<Grid>();
        Ui.SetActive(false);
        Ui2.SetActive(false);

        // Ensure the line is behind the tilemap
        TilemapRenderer tilemapRenderer = drawingTilemap.GetComponent<TilemapRenderer>();
        if (tilemapRenderer != null)
        {
            lineRendererPrefab.sortingOrder = tilemapRenderer.sortingOrder - 1;
            holeLineRendererPrefab.sortingOrder = tilemapRenderer.sortingOrder - 1; // Set sorting order for hole line renderer
        }
    }

    private void Update()
    {
    Vector3Int mousePos = GetMousePosition();

    if (message != true)
    {
        if (!mousePos.Equals(previousMousePos))
        {
            highlightTilemap.SetTile(previousMousePos, null);
            if (!holeMode)
            {
                if (cameraController.IsWithinPlacementRadius(drawingTilemap.GetCellCenterWorld(mousePos)))
                {
                    highlightTilemap.SetTile(mousePos, hoverTile);
                }
                else
                {
                    highlightTilemap.SetTile(mousePos, hoverOutofBonds);
                    Debug.Log("Point is outside the allowed placement radius.");
                }
            }
            else
            {
                highlightTilemap.SetTile(mousePos, hoverTileHole);
            }
            previousMousePos = mousePos;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldMousePos = drawingTilemap.GetCellCenterWorld(mousePos);
            if (cameraController.IsWithinPlacementRadius(worldMousePos))
            {
                Vector3Int? existingPoint = FindExistingPoint(mousePos);
                if (existingPoint.HasValue)
                {
                    mousePos = existingPoint.Value;
                    if (vectorPointsList.Count > 2 && mousePos == vectorPointsList[0])
                    {
                        message = true;
                        Debug.Log("completado");
                        UpdateLine();
                        
                        if (!holeMode)
                        {
                            // Calcular el área de la figura inicial
                            areaInicial = CalculatePolygonArea(vectorPointsList);
                            Debug.Log("Área inicial: " + areaInicial + " unidades cuadradas");
                        }
                        
                        if (holeMode)
                        {
                            // Calcular el área del hueco si está en modo hueco
                            areaHueco = CalculatePolygonArea(vectorPointsList);
                            Debug.Log("Área del hueco: " + areaHueco + " unidades cuadradas");
                        }
                        
                        return;
                    }
                }

                if (!vectorPointsList.Contains(mousePos))
                {
                    PlaceVectorPoint(mousePos);
                    UpdateLine();
                }
            }
            else
            {
                Debug.LogWarning("Point is outside the allowed placement radius.");
            }
        }
    }

    if (message)
    {
        Ui.SetActive(true);
        highlightTilemap.SetTile(previousMousePos, null);
    }
    else
    {
        Ui.SetActive(false);
    }
}

    private Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }

    private void PlaceVectorPoint(Vector3Int cellPosition)
    {
        if (holeMode)
        {
            drawingTilemap.SetTile(cellPosition, vectorPointTileHole);
        }
        else
        {
            drawingTilemap.SetTile(cellPosition, vectorPointTile);
        }
        vectorPointsList.Add(cellPosition);
        // Asignar el primer punto a cameraController.firstVectorPoint
        if (vectorPointsList.Count == 1)
        {
            cameraController.firstVectorPoint = drawingTilemap.GetCellCenterWorld(cellPosition);
        }
    }

    private Vector3Int? FindExistingPoint(Vector3Int cellPosition)
    {
        Vector3 worldPosition = drawingTilemap.GetCellCenterWorld(cellPosition);
        float cellSize = drawingTilemap.cellSize.x;

        foreach (Vector3Int point in vectorPointsList)
        {
            Vector3 pointWorldPosition = drawingTilemap.GetCellCenterWorld(point);
            float distance = Vector3.Distance(worldPosition, pointWorldPosition);

            if (distance <= cellSize * 0.99f)
            {
                return point;
            }
        }
        return null;
    }

    private void UpdateLine()
    {
        if (holeMode)
        {
            if (currentHoleLine == null)
            {
                currentHoleLine = Instantiate(holeLineRendererPrefab, Vector3.zero, Quaternion.identity);
            }

            int pointCount = vectorPointsList.Count;
            if (message && pointCount > 1)
            {
                currentHoleLine.positionCount = pointCount + 1;
                for (int i = 0; i < pointCount; i++)
                {
                    currentHoleLine.SetPosition(i, drawingTilemap.GetCellCenterWorld(vectorPointsList[i]));
                }
                currentHoleLine.SetPosition(pointCount, drawingTilemap.GetCellCenterWorld(vectorPointsList[0]));
            }
            else
            {
                currentHoleLine.positionCount = pointCount;
                for (int i = 0; i < pointCount; i++)
                {
                    currentHoleLine.SetPosition(i, drawingTilemap.GetCellCenterWorld(vectorPointsList[i]));
                }
            }
        }
        else
        {
            if (currentLine == null)
            {
                currentLine = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
            }

            int pointCount = vectorPointsList.Count;
            if (message && pointCount > 1)
            {
                currentLine.positionCount = pointCount + 1;
                for (int i = 0; i < pointCount; i++)
                {
                    currentLine.SetPosition(i, drawingTilemap.GetCellCenterWorld(vectorPointsList[i]));
                }
                currentLine.SetPosition(pointCount, drawingTilemap.GetCellCenterWorld(vectorPointsList[0]));
            }
            else
            {
                currentLine.positionCount = pointCount;
                for (int i = 0; i < pointCount; i++)
                {
                    currentLine.SetPosition(i, drawingTilemap.GetCellCenterWorld(vectorPointsList[i]));
                }
            }
        }
    }

    public void GenerateMesh()
    {
        Ui2.SetActive(true);
        if (!progressTracker.IsGenerating)
        {
            StartCoroutine(GenerateMeshCoroutine());
        }
    }

    public void generateFinalShape()
    {
        // Verificar si las áreas se calcularon correctamente
        if (areaInicial <= 0)
        {
            Debug.LogError("El área inicial no se calculó correctamente o es 0.");
            return;
        }

        if (areaHueco <= 0)
        {
            Debug.LogError("El área del hueco no se calculó correctamente o es 0.");
            return;
        }

        // Realizar la resta
        areatotal = areaInicial - areaHueco;

        // Llamar al método de sustracción de mallas
        meshSubtractor.SubtractMesh(extrudedObject, extrudedHole);

        Debug.Log("Fin de generateFinalShape, con un area de " + areatotal + " unidades cuadradas");
    }

    public void createcube()
    {
        cubecreator();
    }
    public void cubecreator()
    {

        // Define the vertices of the cube with coordinates in the range of -5 to 5
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-5, 5, 5),
            new Vector3(5, 5, 5),
            new Vector3(5, -5, 5),
            new Vector3(-5, -5, 5),
        };

        // Use CreateShapeFromPolygon to generate the mesh
        // pbMesh.CreateShapeFromPolygon(vertices, 10.0f, false);
        //
        // pbMesh.ToMesh();
        // pbMesh.Refresh();
    }
    private IEnumerator GenerateMeshCoroutine()
    {
        progressTracker.StartGeneration();

        if (vectorPointsList.Count < 3)
        {
            Debug.LogWarning("Not enough points to generate a mesh.");
            progressTracker.CompleteGeneration();
            yield break;
        }

        Vector2 min = drawingTilemap.GetCellCenterWorld(vectorPointsList[0]);
        Vector2 max = min;
        foreach (Vector3Int point in vectorPointsList)
        {
            Vector2 worldPoint = drawingTilemap.GetCellCenterWorld(point);
            min = Vector2.Min(min, worldPoint);
            max = Vector2.Max(max, worldPoint);
        }

        float width = max.x - min.x;
        float height = max.y - min.y;
        Vector2 center = (min + max) / 2f;

        // Define vertices
        Vector3[] vertices = vectorPointsList
            .Select(p => new Vector3(drawingTilemap.GetCellCenterWorld(p).x - center.x, 
                drawingTilemap.GetCellCenterWorld(p).y - center.y, 
                0))
            .ToArray();
        
        // Assign the generated mesh to the appropriate object
        if (!holeMode)
        {
            vertices = EnsureClockwiseOrder(vertices);
            pbMeshObj.CreateShapeFromPolygon(vertices, 2.0f, false);
            extrudedObject.GetComponent<MeshRenderer>().material = shapeMaterial;
            extrudedObject.transform.position = center;
            holeMode = true;
        }
        else
        {
            vertices = EnsureClockwiseOrder(vertices);
            pbMeshHole.CreateShapeFromPolygon(vertices, 2.0f, false);
            extrudedHole.GetComponent<MeshRenderer>().material = holeMaterial;
            extrudedHole.transform.position = center;
        }

        ResetDrawing();

        progressTracker.CompleteGeneration();

        Debug.Log($"Mesh generated at position: {center}, with width: {width}, height: {height}");
        Ui2.SetActive(false);
    }

    public void ResetDrawing()
    {
        message = false;
        vectorPointsList.Clear();
        if (currentLine != null)
        {
            Destroy(currentLine.gameObject);
            currentLine = null;
        }
        if (currentHoleLine != null)
        {
            Destroy(currentHoleLine.gameObject);
            currentHoleLine = null;
        }
        drawingTilemap.ClearAllTiles();
        highlightTilemap.ClearAllTiles();
    }
    
    private Vector3[] EnsureClockwiseOrder(Vector3[] vertices)
    {
        float sum = 0f;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 current = vertices[i];
            Vector3 next = vertices[(i + 1) % vertices.Length];
            sum += (next.x - current.x) * (next.y + current.y);
        }

        // Si el área es negativa, los vértices están en sentido antihorario, así que los invertimos
        if (sum > 0)
        {
            System.Array.Reverse(vertices);
        }

        return vertices;
    }

    private float CalculatePolygonArea(List<Vector3Int> points)
    {
        if (points.Count < 3)
        {
            Debug.LogWarning("No hay suficientes puntos para calcular el área.");
            return 0f;
        }

        float area = 0f;
        int n = points.Count;

        for (int i = 0; i < n; i++)
        {
            Vector2 current = (Vector2)drawingTilemap.GetCellCenterWorld(points[i]);
            Vector2 next = (Vector2)drawingTilemap.GetCellCenterWorld(points[(i + 1) % n]);

            area += current.x * next.y - next.x * current.y;
        }

        area = Mathf.Abs(area) / 2f;
        return area;
    }
}