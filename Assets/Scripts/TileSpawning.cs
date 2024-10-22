using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using Random = UnityEngine.Random;

//A row of tiles
[System.Serializable]
public class TileRow
{
    public TileBase[] tiles;
}

//A set of rows. This is used to create a set 3d grid of tiles for the port or other fixed structures
[System.Serializable]
public class TileLayer
{
    public TileRow[] rows;
}

public class TileSpawning : MonoBehaviour
{
    public Grid grid;
    public Tilemap waterTilemap;
    public Tilemap baseTilemap;
    public Tilemap decorationTilemap;
    //Water tile
    public TileBase waterTile;
    public TileBase landTile;
    public TileBase coastlineTile;
    public TileBase[] decorationTiles;

    //3d grid of tiles for the port
    public TileLayer portTiles;

    public Vector3Int bottomLeftCell;
    public Vector3Int topRightCell;
    public static TileSpawning Instance;

    public List<Vector3Int> coastLines;
    private Camera mainCamera;

    private float nextPortTimer = 0;
    private bool portSpawning = false;
    private int portIndex = 0;

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
        getCorners();
        coastLines = new List<Vector3Int> { topRightCell, topRightCell - new Vector3Int(1, 0, 0) }; //Hardcoding the rightmost two columns of the screen to be coast
    }

    void Start()
    {
        fillStartWater();
        spawnStartCoast(coastLines);
        spawnStartDecorations(coastLines);
    }

    private void Update()
    {
        //Move the grid down 2 unit per second
        grid.transform.position -= new Vector3(0, 2 * Time.deltaTime, 0);
        //Every x amount of time start to make a port
        if (Time.time > nextPortTimer)
        {
            //Spawn port
            portSpawning = true;
            nextPortTimer = Time.time + Random.Range(60, 100);
        }
        spawnTopRow(coastLines);
        despawnBottomRow();
    }

    //Get the corners of the camera's view in world coordinates
    private void getCorners()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        //Get the corners of the camera's view in world coordinates
        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

        //Convert world coordinates to cell positions in the Tilemap
        bottomLeftCell = waterTilemap.WorldToCell(bottomLeft);
        topRightCell = waterTilemap.WorldToCell(topRight);
    }

    //Fill the Tilemap with water tiles within the calculated bounds
    private void fillStartWater()
    {
        //Fill the Tilemap with water tiles within the calculated bounds
        for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
        {
            for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
            {
                waterTilemap.SetTile(new Vector3Int(x, y, 0), waterTile);
                //Rotate the water tiles randomly
                waterTilemap.SetTransformMatrix(new Vector3Int(x, y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
        }
    }

    //Spawn layers of coast on the inputted x lines of the of the screen
    private void spawnStartCoast(List<Vector3Int> xLines)
    {
        foreach (Vector3Int cell in xLines)
        {
            for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
            {
                baseTilemap.SetTile(new Vector3Int(cell.x, y, 0), landTile);
            }
            baseTilemap.SetTile(cell, landTile);
        }
    }

    //Spawn layers of decorations on the inputted x lines of the of the screen
    private void spawnStartDecorations(List<Vector3Int> xLines)
    {
        foreach (Vector3Int cell in xLines)
        {
            for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
            {
                if (Random.Range(0, 100) < 30)
                {
                    decorationTilemap.SetTile(new Vector3Int(cell.x, y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                    decorationTilemap.SetTransformMatrix(new Vector3Int(cell.x, y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
                }
            }
        }
    }

    //Despawn the bottom row of tiles if they are out of view
    private void despawnBottomRow()
    {
        if (mainCamera == null)
        {
            return;
        }

        // Get the bottom edge of the camera's view in world coordinates
        Vector3 bottomEdge = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, mainCamera.nearClipPlane));
        Vector3Int bottomEdgeCell = waterTilemap.WorldToCell(bottomEdge);

        // Check if the bottom row of tiles is out of view and remove them
        if (bottomEdgeCell.y > bottomLeftCell.y)
        {
            for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
            {
                waterTilemap.SetTile(new Vector3Int(x, bottomLeftCell.y, 0), null);
                baseTilemap.SetTile(new Vector3Int(x, bottomLeftCell.y, 0), null);
                decorationTilemap.SetTile(new Vector3Int(x, bottomLeftCell.y, 0), null);
            }
            bottomLeftCell.y++;
        }
    }

    //Spawn the top row of tiles if they are going to be in view
    //Also spawn coast and decorations on the inputted x lines
    private void spawnTopRow(List<Vector3Int> xLines)
    {
        if (mainCamera == null)
        {
            return;
        }

        // Get the top edge of the camera's view in world coordinates
        Vector3 topEdge = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, mainCamera.nearClipPlane));
        Vector3Int topEdgeCell = waterTilemap.WorldToCell(topEdge);

        // Check if the top row of tiles is going to be in view and spawn them
        if (topEdgeCell.y + 1 > topRightCell.y)
        {
            topRightCell.y++;
            for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
            {
                waterTilemap.SetTile(new Vector3Int(x, topRightCell.y, 0), waterTile);
                waterTilemap.SetTransformMatrix(new Vector3Int(x, topRightCell.y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
            if (portSpawning)
            {
                int column = topRightCell.x;
                //Spawn the port tiles of the index starting from the right of the screen using the portIndex to tell what row
                for (int i = portTiles.rows[portIndex].tiles.Length - 1; i >= 0; i--) //Because I put the port tiles in the array backwards and this is easier than changing them all
                {
                    baseTilemap.SetTile(new Vector3Int(column, topRightCell.y, 0), portTiles.rows[portIndex].tiles[i]);
                    column--;
                }
                portIndex++;
                if (portIndex >= portTiles.rows.Length)
                {
                    portSpawning = false;
                    portIndex = 0;
                }
            }
            else
            {
                foreach (Vector3Int cell in xLines)
                {
                    baseTilemap.SetTile(new Vector3Int(cell.x, topRightCell.y, 0), landTile);
                    if (Random.Range(0, 100) < 30)
                    {
                        decorationTilemap.SetTile(new Vector3Int(cell.x, topRightCell.y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                    }
                }
            }
        }
    }
}
