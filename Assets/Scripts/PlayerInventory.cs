using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    public List<LootAmount> inventory = new List<LootAmount>();

    private int coins = 0;

    public int Coins
    {
        get => coins;
        set
        {
            coins = value;
        }
    }

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
        if (lootType == LootType.Copper || lootType == LootType.Silver || lootType == LootType.Gold)
        {
            Coins += (int)lootType * amount;
            if (amount > 0)
            {
                MainUI.Instance.updateDisplay(lootType, amount);
            }
            return;
        }
        //Check if there is enough space in the player's inventory. If not, return
        checkCapacity(lootType, ref amount);
        if (amount == 0)
        {
            return;
        }
        MainUI.Instance.updateDisplay(lootType, amount);
        //Check if the player already has this type of loot
        foreach (LootAmount loot in inventory)
        {
            if (loot.lootType == lootType)
            {
                loot.amount += amount;
                return;
            }
        }
        //If the player doesn't have this type of loot, add it to the inventory
        inventory.Add(new LootAmount { lootType = lootType, amount = amount });
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
    /// Pays the required amount or returns false if the player doesn't have enough coins
    /// </summary>
    /// <param name="cost">The cost of the payment</param>
    /// <returns>True if the payment was successful, false otherwise</returns>
    public bool PayWithCoins(int cost)
    {
        if (Coins < cost)
        {
            return false;
        }
        Coins -= cost;
        return true;
    }

    /// <summary>
    /// Checks if there is enough space in the player's inventory for the specified amount of loot
    /// </summary>
    /// <param name="lootType">The type of loot to check</param>
    /// <param name="amount">The amount of loot to check. Modified to the amount that will fit</param>
    private void checkCapacity(LootType lootType, ref int amount)
    {
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
    /// Clears all loot from the player's inventory
    /// </summary>
    public void ClearInventory()
    {
        inventory.Clear();
    }

    /// <summary>
    /// Returns the amount of loot of the specified type in the player's inventory
    /// </summary>
    /// <param name="lootType">The type of loot to get</param>
    public int GetLootAmount(LootType lootType) => inventory.Where(loot => loot.lootType == lootType).Sum(loot => loot.amount);

    /// <summary>
    /// Returns the amount of inventory space used
    /// </summary>
    private int GetInventorySize => inventory.Where(loot => loot.lootType != LootType.IdleLoot).Sum(loot => loot.amount);

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

    /// <summary>
    /// Returns a string with the amount of coins the player has
    /// </summary>
    public string CoinDisplay => "Gold: " + Coins / 10000 + "- Silver: " + (Coins % 10000) / 100 + "- Copper: " + Coins % 100;

}
