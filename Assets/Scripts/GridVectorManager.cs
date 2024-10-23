using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    private GameObject extrudedObject;
    private GameObject extrudedHole;

    private Grid grid;
    private List<Vector3Int> vectorPointsList = new List<Vector3Int>();
    private LineRenderer currentLine;
    private LineRenderer currentHoleLine; // New LineRenderer instance for hole mode
    private Vector3Int previousMousePos = new Vector3Int();
    private bool message = false;
    private bool holeMode = false;

    private void Start()
    {
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

        Mesh mesh = new Mesh();
        Vector3[] vertices = vectorPointsList.Select(p => (Vector3)drawingTilemap.GetCellCenterWorld(p) - (Vector3)center).ToArray();
        int[] triangles = GenerateTriangles(vertices);
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        if (!holeMode)
        {
            Vector3 nCenter = new Vector3(center.x, center.y, 1.0f);
            extrudedObject = MeshExtruder.ExtrudeMesh(mesh, nCenter, 1.0f, shapeMaterial, "ExtrudedMesh");
            holeMode = true;
        }
        else
        {
            Vector3 holeCenter = new Vector3(center.x, center.y, 0.9f);
            extrudedHole = MeshExtruder.ExtrudeMesh(mesh, holeCenter, 1.0f, holeMaterial, "ExtrudedHole");
            //finalShape.GenerateMeshForAnimation(mesh, center, "HoleShape", holeMaterial, 1 );
        }

        ResetDrawing();

        progressTracker.CompleteGeneration();

        Debug.Log($"Mesh generated at position: {center}, with width: {width}, height: {height}");
        Ui2.SetActive(false);

        
    }
    
    public void generateFinalShape()
    {
        meshSubtractor.SubtractMesh(extrudedObject,extrudedHole);
    }

    private int[] GenerateTriangles(Vector3[] vertices)
    {
        List<int> indices = new List<int>();

        int n = vertices.Length;
        Debug.Log($"Number of points in the shape: {n}");
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area(vertices) > 0)
        {
            for (int v = 0; v < n; v++) V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++) V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0) return indices.ToArray();

            int u = v; if (nv <= u) u = 0;
            v = u + 1; if (nv <= v) v = 0;
            int w = v + 1; if (nv <= w) w = 0;

            if (Snip(vertices, u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u]; b = V[v]; c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                for (s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t]; nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area(Vector3[] vertices)
    {
        int n = vertices.Length;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            A += vertices[p].x * vertices[q].y - vertices[q].x * vertices[p].y;
        }
        return (A * 0.5f);
    }

    private bool Snip(Vector3[] vertices, int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector3 A = vertices[V[u]];
        Vector3 B = vertices[V[v]];
        Vector3 C = vertices[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w)) continue;
            Vector3 P = vertices[V[p]];
            if (InsideTriangle(A, B, C, P)) return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
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
}