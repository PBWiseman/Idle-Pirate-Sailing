using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[System.Serializable]
public class DebrisRarity
{
    public GameObject debrisPrefab;
    public int ChanceToSpawn;
}

public class DebrisSpawning : MonoBehaviour
{
    public List<GameObject> debrisObjects = new List<GameObject>();
    private List<int> spawnedRows = new List<int>();
    public static DebrisSpawning Instance;
    public List<DebrisRarity> debrisPrefabs = new List<DebrisRarity>();

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
        //If there are less than 10 debris objects and there isn't one in this row, have a 1 in 1000 chance of spawning one
        if (debrisObjects.Count < 10 && !spawnedRows.Contains(TileSpawning.Instance.topRightCell.y + 1) && Random.Range(0, 100) == 0)
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
        //If the oldest debris object is below the bottom of the screen, remove it
        if (debrisObjects.Count() != 0 && debrisObjects[0] != null && debrisObjects[0].transform.localPosition.y < TileSpawning.Instance.bottomLeftCell.y)
        {
            GameObject debris = debrisObjects[0];
            debrisObjects.Remove(debris);
            spawnedRows.Remove(spawnedRows.Min());
            Destroy(debris);
        }
    }

    private void spawnDebris(Vector3Int cell)
    {
        //Select a random debris prefab based on rarity. If object A has a 15 ChanceToSpawn and object B has a 5 ChanceToSpawn, object A will be selected 3/4 of the time
        int totalChance = 0;
        foreach (DebrisRarity debris in debrisPrefabs)
        {
            totalChance += debris.ChanceToSpawn;
        }
        int randomChance = Random.Range(0, totalChance);
        GameObject debrisPrefab = null;
        foreach (DebrisRarity debris in debrisPrefabs)
        {
            randomChance -= debris.ChanceToSpawn;
            if (randomChance <= 0)
            {
                debrisPrefab = debris.debrisPrefab;
                break;
            }
        }
        GameObject spawnedDebris = Instantiate(debrisPrefab, TileSpawning.Instance.baseTilemap.GetCellCenterWorld(cell), Quaternion.identity);
        spawnedDebris.transform.SetParent(TileSpawning.Instance.grid.transform);
        debrisObjects.Add(spawnedDebris);
        spawnedRows.Add(cell.y);
    }
}
