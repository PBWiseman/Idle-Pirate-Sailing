using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    public List<LootAmount> inventory = new List<LootAmount>();

    public int maxInventorySize = 50; //Starts at 50. Can be upgraded
    public int maxIdleInventorySize = 50; //Starts at 50. Can be upgraded

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

    /// <summary>
    /// Adds the specified amount of loot to the player's inventory
    /// </summary>
    /// <param name="lootType">The type of loot to add</param>
    /// <param name="amount">The amount of loot to add</param>
    public void AddLoot(LootType lootType, int amount)
    {
        //Check if there is enough space in the player's inventory. If not, return
        checkCapacity(lootType, ref amount);
        if (amount == 0)
        {
            return;
        }
        //Check if the player already has this type of loot
        foreach (LootAmount loot in inventory)
        {
            if (loot.lootType == lootType)
            {
                loot.amount += amount;
                ConvertCoins(lootType);
                MainUI.Instance.updateDisplay(lootType, amount);
                return;
            }
        }
        //If the player doesn't have this type of loot, add it to the inventory
        inventory.Add(new LootAmount { lootType = lootType, amount = amount });
        ConvertCoins(lootType);
        MainUI.Instance.updateDisplay(lootType, amount);
    }

    /// <summary>
    /// Removes the specified amount of loot from the player's inventory
    /// </summary>
    /// <param name="lootType">The type of loot to remove</param>
    /// <param name="amount">The amount of loot to remove</param>
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

    /// <summary>
    /// Returns the amount of loot of the specified type in the player's inventory
    /// </summary>
    /// <param name="lootType">The type of loot to get</param>
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
    /// <param name="lootType">The type of loot to set</param>
    /// <param name="amount">The amount of loot to set</param>
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
    /// <param name="lootType">Loot type. Will convert if it is a small coin</param>
    private void ConvertCoins(LootType lootType)
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

    /// <summary>
    /// Checks if there is enough space in the player's inventory for the specified amount of loot
    /// </summary>
    /// <param name="lootType">The type of loot to check</param>
    /// <param name="amount">The amount of loot to check. Modified to the amount that will fit</param>
    private void checkCapacity(LootType lootType, ref int amount)
    {
        //Coins don't take up inventory space
        if (lootType == LootType.Copper || lootType == LootType.Silver || lootType == LootType.Gold)
        {
            return;
        }

        int spaceLeft = 0;
        //Check if the loot is idle loot and set the space left accordingly
        if (lootType == LootType.IdleLoot)
        {
            spaceLeft = maxIdleInventorySize - GetIdleInventorySize;
        }
        else
        {
            spaceLeft = maxInventorySize - GetInventorySize;
        }
        //If there is not enough space, set the amount to the space left
        if (spaceLeft < amount)
        {
            amount = spaceLeft;
        }
    }

    /// <summary>
    /// Returns the amount of inventory space used
    /// </summary>
    private int GetInventorySize => inventory.Where(loot => loot.lootType != LootType.IdleLoot &&
                                loot.lootType != LootType.Copper &&
                                loot.lootType != LootType.Silver &&
                                loot.lootType != LootType.Gold)
                                .Sum(loot => loot.amount);

    /// <summary>
    /// Returns the amount of idle inventory space used
    /// </summary>
    private int GetIdleInventorySize => inventory.Where(loot => loot.lootType == LootType.IdleLoot).Sum(loot => loot.amount);

    /// <summary>
    /// Returns the amount of inventory space used as a string
    /// </summary>
    public string CapacityRegular => GetInventorySize + "/" + maxInventorySize;

    /// <summary>
    /// Returns the amount of idle inventory space used as a string
    /// </summary>
    public string CapacityIdle => GetIdleInventorySize + "/" + maxIdleInventorySize;
}
