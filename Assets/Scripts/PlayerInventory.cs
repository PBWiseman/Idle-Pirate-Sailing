using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<LootAmount> inventory = new List<LootAmount>();

    public static PlayerInventory Instance;
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
    
    public void AddLoot(LootType lootType, int amount)
    {
        //Check if the player already has this type of loot
        foreach (LootAmount loot in inventory)
        {
            if (loot.lootType == lootType)
            {
                loot.amount += amount;
                ConvertCoins(lootType);
                return;
            }
        }
        //If the player doesn't have this type of loot, add it to the inventory
        inventory.Add(new LootAmount { lootType = lootType, amount = amount });
        ConvertCoins(lootType);
    }

    public void RemoveLoot(LootType lootType, int amount)
    {
        //Check if the player has this type of loot
        foreach (LootAmount loot in inventory)
        {
            if (loot.lootType == lootType)
            {
                loot.amount -= amount;
                if (loot.amount <= 0)
                {
                    inventory.Remove(loot);
                }
                return;
            }
        }
    }

    public int GetLootAmount(LootType lootType)
    {
        foreach (LootAmount loot in inventory)
        {
            if (loot.lootType == lootType)
            {
                return loot.amount;
            }
        }
        return 0;
    }

    /// <summary>
    /// Sets the amount of loot of the specified type in the player's inventory. Mainly used for testing or coins after payment
    /// </summary>
    public void SetLootAmount(LootType lootType, int amount)
    {
        foreach (LootAmount loot in inventory)
        {
            if (loot.lootType == lootType)
            {
                loot.amount = amount;
                return;
            }
        }
        inventory.Add(new LootAmount { lootType = lootType, amount = amount });
    }

    /// <summary>
    /// Converts the player's copper coins to silver and silver coins to gold
    /// </summary>
    public void ConvertCoins(lootType lootType)
    {
        if (lootType != LootType.Copper && lootType != LootType.Silver)
        {
            return;
        }
        while (GetLootAmount(LootType.Copper) >= 100)
        {
            RemoveLoot(LootType.Copper, 100);
            AddLoot(LootType.Silver, 1);
        }
        while (GetLootAmount(LootType.Silver) >= 100)
        {
            RemoveLoot(LootType.Silver, 100);
            AddLoot(LootType.Gold, 1);
        }
    }

    /// <summary>
    /// Pays the required amount or returns false if the player doesn't have enough coins
    /// </summary>
    /// <param name="goldCost">The amount of gold coins required</param>
    /// <param name="silverCost">The amount of silver coins required</param>
    /// <param name="copperCost">The amount of copper coins required</param>
    /// <returns>True if enough coins and the payment was successful, false otherwise</returns>
    public bool PayWithCoins(int goldCost, int silverCost, int copperCost)
    {
        //Convert all to copper for easier comparison
        int copperOwned = (GetLootAmount(LootType.Gold) * 10000) + (GetLootAmount(LootType.Silver) * 100) + GetLootAmount(LootType.Copper);
        int totalCopperCost = (goldCost * 10000) + (silverCost * 100) + copperCost;
        
        //Pay the amount and return false if the player doesn't have enough coins
        if (copperOwned < totalCopperCost)
        {
            return false;
        }
        copperOwned -= totalCopperCost;
        
        //Update coins
        SetLootAmount(LootType.Gold, copperOwned / 10000);
        SetLootAmount(LootType.Silver, (copperOwned % 10000) / 100);
        SetLootAmount(LootType.Copper, copperOwned % 100);

        return true;
    }
}
