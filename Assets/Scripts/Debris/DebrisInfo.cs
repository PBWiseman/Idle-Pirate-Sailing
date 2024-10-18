using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootGeneration
{
    public LootType lootType;
    //Inclusive
    public int min;
    //Inclusive
    public int max;
}

public class DebrisInfo : MonoBehaviour
{
    public LootTable loot;
    public List<Sprite> debrisSprites;
    public SpriteRenderer spriteRenderer;
    public List<LootGeneration> lootGeneration;
    // Start is called before the first frame update
    void Start()
    {
        //Set random sprite
        spriteRenderer.sprite = debrisSprites[Random.Range(0, debrisSprites.Count)];
        //Set random rotation
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        loot = new LootTable();
        foreach (LootGeneration lootGen in lootGeneration)
        {
            loot.AddLoot(lootGen.lootType, lootGen.min, lootGen.max);
        }
    }
}
