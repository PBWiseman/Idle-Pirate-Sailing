/// <summary>
/// Reworking the system to use a preset map and free movement
/// Just having it spawn water tiles in all places in view for the moment
/// </summary>
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using Random = UnityEngine.Random;

//A row of tiles
[Serializable]
public class TileRow
{
    public TileBase[] tiles;
}

//A set of rows. This is used to create a set 3d grid of tiles for the port or other fixed structures
[Serializable]
public class TileLayer
{
    public TileRow[] rows;
}

public class TileSpawning : MonoBehaviour
{
    public Grid grid;
    public Tilemap waterTilemap;
    public TileBase waterTile;
    public static TileSpawning Instance;
    private Camera mainCamera;
    private HashSet<Vector3Int> currentlyVisibleTiles = new HashSet<Vector3Int>();
    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        int refreshRate;
        //Cap this at the lesser of the screen's refresh rate and 60fps
        //TODO: Fix this to be the new way of doing it? Apparently this is deprecated
        if (Screen.currentResolution.refreshRate < 0)
        {
            refreshRate = 60;
        }
        else
        {
            refreshRate = Math.Min(Screen.currentResolution.refreshRate, 60);
        }
        Application.targetFrameRate = refreshRate;
        mainCamera = Camera.main;
    }

    void Start()
    {

    }

    private void Update()
    {
        if (mainCamera == null) return;

        // Only update if the camera has moved or rotated
        if (mainCamera.transform.position != lastCameraPosition || mainCamera.transform.rotation != lastCameraRotation)
        {
            spawnTiles();
            lastCameraPosition = mainCamera.transform.position;
            lastCameraRotation = mainCamera.transform.rotation;
        }
    }

    private void spawnTiles()
    {
        HashSet<Vector3Int> newVisibleTiles = GetVisibleTiles();

        // Tiles to spawn
        foreach (var cell in newVisibleTiles)
        {
            if (!currentlyVisibleTiles.Contains(cell))
            {
                waterTilemap.SetTile(cell, waterTile);
                //waterTilemap.SetTransformMatrix(cell, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
        }

        // Tiles to despawn
        foreach (var cell in currentlyVisibleTiles)
        {
            if (!newVisibleTiles.Contains(cell))
            {
                waterTilemap.SetTile(cell, null);
            }
        }

        currentlyVisibleTiles = newVisibleTiles;
    }

    //Get the corners of the camera's view in world coordinates
    private HashSet<Vector3Int> GetVisibleTiles()
    {
        if (mainCamera == null || waterTilemap == null)
        {
            return new HashSet<Vector3Int>();
        }

        // Get the four corners of the camera's view in world space
        Vector3[] worldCorners = new Vector3[4];
        worldCorners[0] = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)); // Bottom Left
        worldCorners[1] = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)); // Bottom Right
        worldCorners[2] = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane)); // Top Right
        worldCorners[3] = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane)); // Top Left

        // Convert to cell positions
        Vector3Int[] cellCorners = new Vector3Int[4];
        for (int i = 0; i < 4; i++)
        {
            cellCorners[i] = waterTilemap.WorldToCell(worldCorners[i]);
        }

        // Find bounds (min/max x and y)
        int minX = Mathf.Min(cellCorners[0].x, cellCorners[1].x, cellCorners[2].x, cellCorners[3].x);
        int maxX = Mathf.Max(cellCorners[0].x, cellCorners[1].x, cellCorners[2].x, cellCorners[3].x);
        int minY = Mathf.Min(cellCorners[0].y, cellCorners[1].y, cellCorners[2].y, cellCorners[3].y);
        int maxY = Mathf.Max(cellCorners[0].y, cellCorners[1].y, cellCorners[2].y, cellCorners[3].y);

        // Collect all cells within the rectangle that are actually inside the camera frustum
        HashSet<Vector3Int> visibleCells = new HashSet<Vector3Int>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3 cellCenter = waterTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                if (GeometryUtility.TestPlanesAABB(planes, new Bounds(cellCenter, Vector3.one)))
                {
                    visibleCells.Add(new Vector3Int(x, y, 0));
                }
            }
        }
        return visibleCells;
    }

}
