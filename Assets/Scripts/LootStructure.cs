using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The type of loot that can be dropped
/// Note: The values of the enum are the values of the loot type
/// </summary>
public enum LootType
{
    Fabric,
    Wood,
    Metal,
    Gold,
    Silver,
    Copper,
    IdleLoot
}

public class LootAmount
{
    public LootType lootType;
    public int amount;
}

[System.Serializable]
public class LootTable
{
    public List<LootAmount> lootTable = new List<LootAmount>();

    /// <summary>
    /// Adds a loot type to the loot table. Set min and max to the same value for a fixed amount of loot.
    /// Min and Max are inclusive.
    /// </summary>
    /// <param name="lootType">The type of loot to add</param>
    /// <param name="min">The minimum amount of loot to add, inclusive</param>
    /// <param name="max">The maximum amount of loot to add, inclusive</param>
    public void AddLoot(LootType lootType, int min, int max)
    {
        //Note: Add 1 to max to make it inclusive
        lootTable.Add(new LootAmount { lootType = lootType, amount = Random.Range(min, max + 1) });
    }
}