using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSpawning : MonoBehaviour
{
    public Grid grid;
    public Tilemap waterTilemap;
    public Tilemap baseTilemap;
    public Tilemap decorationTilemap;
    //Water tile
    public TileBase waterTile;
    public TileBase landTile;
    public TileBase[] decorationTiles;

    private Vector3Int bottomLeftCell;
    private Vector3Int topRightCell;

    void Start()
    {
        getCorners();
        fillWater();
        spawnCoast();
        spawnDecorations();
    }

    private void Update()
    {
        //Move the grid down 2 unit per second
        grid.transform.position -= new Vector3(0, 2 * Time.deltaTime, 0);
        despawnBottomRow();
        spawnTopRow();
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
    private void fillWater()
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

    //Spawn 2 layers of coast on the left and right of the screen
    private void spawnCoast()
    {
        //Spawn coast on the left and right of the screen
        for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
        {
            baseTilemap.SetTile(new Vector3Int(bottomLeftCell.x, y, 0), landTile);
        }
        for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
        {
            baseTilemap.SetTile(new Vector3Int(topRightCell.x, y, 0), landTile);
        }
        for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
        {
            baseTilemap.SetTile(new Vector3Int(bottomLeftCell.x + 1, y, 0), landTile);
        }
        for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
        {
            baseTilemap.SetTile(new Vector3Int(topRightCell.x - 1, y, 0), landTile);
        }
    }

    //Despawn the bottom row of tiles if they are out of view
    private void despawnBottomRow()
    {
        Camera mainCamera = Camera.main;
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
    private void spawnTopRow()
    {
        Camera mainCamera = Camera.main;
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
            for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
            {
                waterTilemap.SetTile(new Vector3Int(x, topRightCell.y, 0), waterTile);
                waterTilemap.SetTransformMatrix(new Vector3Int(x, topRightCell.y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
            //Spawn coast on the left and right of the screen
            baseTilemap.SetTile(new Vector3Int(bottomLeftCell.x, topRightCell.y, 0), landTile);
            baseTilemap.SetTile(new Vector3Int(topRightCell.x, topRightCell.y, 0), landTile);
            baseTilemap.SetTile(new Vector3Int(bottomLeftCell.x + 1, topRightCell.y, 0), landTile);
            baseTilemap.SetTile(new Vector3Int(topRightCell.x - 1, topRightCell.y, 0), landTile);
            topRightCell.y++;
            if (Random.Range(0, 100) < 30)
            {
                decorationTilemap.SetTile(new Vector3Int(bottomLeftCell.x, topRightCell.y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                decorationTilemap.SetTransformMatrix(new Vector3Int(bottomLeftCell.x, topRightCell.y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
            if (Random.Range(0, 100) < 30)
            {
                decorationTilemap.SetTile(new Vector3Int(topRightCell.x, topRightCell.y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                decorationTilemap.SetTransformMatrix(new Vector3Int(topRightCell.x, topRightCell.y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
            if (Random.Range(0, 100) < 30)
            {
                decorationTilemap.SetTile(new Vector3Int(bottomLeftCell.x + 1, topRightCell.y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                decorationTilemap.SetTransformMatrix(new Vector3Int(bottomLeftCell.x, topRightCell.y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
            if (Random.Range(0, 100) < 30)
            {
                decorationTilemap.SetTile(new Vector3Int(topRightCell.x - 1, topRightCell.y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                decorationTilemap.SetTransformMatrix(new Vector3Int(topRightCell.x, topRightCell.y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
        }
    }

    private void spawnDecorations()
    {
        for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
        {
            if (Random.Range(0, 100) < 30)
            {
                decorationTilemap.SetTile(new Vector3Int(bottomLeftCell.x + 1, y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                decorationTilemap.SetTransformMatrix(new Vector3Int(bottomLeftCell.x, y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
        }
        for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
        {
            if (Random.Range(0, 100) < 30)
            {
                decorationTilemap.SetTile(new Vector3Int(topRightCell.x - 1, y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                decorationTilemap.SetTransformMatrix(new Vector3Int(topRightCell.x, y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
        }
        for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
        {
            if (Random.Range(0, 100) < 30)
            {
                decorationTilemap.SetTile(new Vector3Int(bottomLeftCell.x + 1, y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                decorationTilemap.SetTransformMatrix(new Vector3Int(bottomLeftCell.x, y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
        }
        for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
        {
            if (Random.Range(0, 100) < 30)
            {
                decorationTilemap.SetTile(new Vector3Int(topRightCell.x - 1, y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                decorationTilemap.SetTransformMatrix(new Vector3Int(topRightCell.x, y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
        }
    }
}
