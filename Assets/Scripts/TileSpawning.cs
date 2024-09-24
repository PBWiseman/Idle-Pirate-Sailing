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

    private List<int> coastLines;


    void Start()
    {
        getCorners();
        fillStartWater();
        coastLines = new List<int> { topRightCell.x, topRightCell.x - 1 }; //Hardcoding a list of x lines to fill with coast
        spawnStartCoast(coastLines);
        spawnStartDecorations(coastLines);
    }

    private void Update()
    {
        //Move the grid down 2 unit per second
        grid.transform.position -= new Vector3(0, 2 * Time.deltaTime, 0);
        despawnBottomRow();
        spawnTopRow(coastLines);
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
    private void spawnStartCoast(List<int> xLines)
    {
        foreach (int x in xLines)
        {
            for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
            {
                baseTilemap.SetTile(new Vector3Int(x, y, 0), landTile);
            }
        }
    }

    //Spawn layers of decorations on the inputted x lines of the of the screen
    private void spawnStartDecorations(List<int> xLines)
    {
        foreach (int x in xLines)
        {
            for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
            {
                if (Random.Range(0, 100) < 30)
                {
                    decorationTilemap.SetTile(new Vector3Int(x, y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                    decorationTilemap.SetTransformMatrix(new Vector3Int(x, y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
                }
            }
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
    //Also spawn coast and decorations on the inputted x lines
    private void spawnTopRow(List<int> xLines)
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
            topRightCell.y++;
            for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
            {
                waterTilemap.SetTile(new Vector3Int(x, topRightCell.y, 0), waterTile);
                waterTilemap.SetTransformMatrix(new Vector3Int(x, topRightCell.y, 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), Vector3.one));
            }
            foreach (int x in xLines)
            {
                baseTilemap.SetTile(new Vector3Int(x, topRightCell.y, 0), landTile);
                if (Random.Range(0, 100) < 30)
                {
                    decorationTilemap.SetTile(new Vector3Int(x, topRightCell.y, 0), decorationTiles[Random.Range(0, decorationTiles.Length)]);
                }
            }
        }
    }

}
