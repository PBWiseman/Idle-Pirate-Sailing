using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// The type of loot that can be dropped
/// Note: The values of the enum are the values of the loot type
/// </summary>
[Serializable]
public enum LootType
{
    Fabric = 5,
    Wood = 10,
    Metal = 20,
    Gold = 10000,
    Silver = 100,
    Copper = 1,
    IdleLoot = 0, //Idle loot and inventory are here to make displaying them easier
    Inventory = -1
}
[Serializable]
public class LootAmount
{
    public LootType lootType;
    public int amount;
}

[Serializable]
public class Inventory
{
    public List<LootAmount> inventory = new List<LootAmount>();
    public int coins = 0;
}

[Serializable]
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