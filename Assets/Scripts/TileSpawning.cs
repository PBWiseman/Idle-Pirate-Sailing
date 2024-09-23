using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawning : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            public GameObject tilePrefab; // Assign a prefab with a SpriteRenderer component
    public Sprite[] tileSprites; // Assign the sliced sprites in the Inspector

    void Start()
    {
        for (int i = 0; i < tileSprites.Length; i++)
        {
            GameObject tile = Instantiate(tilePrefab, new Vector3(i, 0, 0), Quaternion.identity);
            tile.GetComponent<SpriteRenderer>().sprite = tileSprites[i];
        }
    }
    }
}
