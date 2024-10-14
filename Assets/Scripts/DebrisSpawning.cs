using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class DebrisSpawning : MonoBehaviour
{

    public Sprite[] debrisSprites;
    public List<GameObject> debrisObjects = new List<GameObject>();
    public GameObject debrisPrefab;
    private List<int> spawnedRows = new List<int>();
    public static DebrisSpawning Instance;

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
    }

    // Update is called once per frame
    void Update()
    {
        //If there are less than 10 debris objects and there isn't one in this row, have a 1 in 50 chance of spawning one
        if (debrisObjects.Count < 10 && !spawnedRows.Contains(TileSpawning.Instance.topRightCell.y + 1) && Random.Range(0, 500) == 0)
        {
            //Add each horizontal cell at the top of the baseTilemap, no matter the size of the grid, to the list of spawn locations
            List<Vector3Int> availableLocations = new List<Vector3Int>();
            for (int x = TileSpawning.Instance.bottomLeftCell.x + 1; x < TileSpawning.Instance.topRightCell.x - 1; x++)
            {
                //If there isn't something in the cell, add it to the list of available locations
                if (TileSpawning.Instance.baseTilemap.GetTile(new Vector3Int(x, TileSpawning.Instance.topRightCell.y, 0)) == null)
                {
                    availableLocations.Add(new Vector3Int(x, TileSpawning.Instance.topRightCell.y + 1, 0));
                }
            }
            
            if (availableLocations.Count > 0)
            {
                spawnDebris(availableLocations[Random.Range(0, availableLocations.Count)]);
            }
        }
        //If the debris is off the screen, destroy it
        foreach (GameObject debris in debrisObjects)
        {
            if (debris.transform.localPosition.y < TileSpawning.Instance.bottomLeftCell.y)
            {
                debrisObjects.Remove(debris);
                Destroy(debris);
                break;
            }
        }
    }

    private void spawnDebris(Vector3Int cell)
    {
        GameObject debris = Instantiate(debrisPrefab, TileSpawning.Instance.baseTilemap.GetCellCenterWorld(cell), Quaternion.identity);
        debris.GetComponent<SpriteRenderer>().sprite = debrisSprites[Random.Range(0, debrisSprites.Length)];
        debris.transform.SetParent(TileSpawning.Instance.grid.transform);
        debris.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        debrisObjects.Add(debris);
        spawnedRows.Add(cell.y);
    }
}
