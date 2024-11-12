using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerInventory : MonoBehaviour
{
    public Inventory Inventory
    {
        get => Saving.Instance.Inventory;
        set => Saving.Instance.Inventory = value;
    }

    public int Coins
    {
        get => Inventory.coins;
        set => Inventory.coins = value;
    }

    public int maxInventorySize
    {
        get 
        {
            Upgrade upgrade = Saving.Instance.Upgrades.GetUpgrade(UpgradeType.InventorySize);
            return upgrade.GetCurrentValue();
        }
    }

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
        //Check if the player already has this type of loot
        foreach (LootAmount loot in Inventory.inventory)
        {
            if (loot.lootType == lootType)
            {
                loot.amount += amount;
                MainUI.Instance.updateDisplay(lootType, amount);
                return;
            }
        }
        //If the player doesn't have this type of loot, add it to the inventory
        Inventory.inventory.Add(new LootAmount { lootType = lootType, amount = amount });
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
        foreach (LootAmount loot in Inventory.inventory)
        {
            if (loot.lootType == lootType)
            {
                loot.amount -= amount;
                if (loot.amount <= 0)
                {
                    Inventory.inventory.Remove(loot);
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
        foreach (LootAmount loot in Inventory.inventory)
        {
            if (loot.lootType == lootType)
            {
                loot.amount = amount;
                return;
            }
        }
        Inventory.inventory.Add(new LootAmount { lootType = lootType, amount = amount });
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
        spaceLeft = maxInventorySize - GetInventorySize;
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
        Inventory.inventory.Clear();
    }

    /// <summary>
    /// Returns the amount of loot of the specified type in the player's inventory
    /// </summary>
    /// <param name="lootType">The type of loot to get</param>
    public int GetLootAmount(LootType lootType) => Inventory.inventory.Where(loot => loot.lootType == lootType).Sum(loot => loot.amount);

    /// <summary>
    /// Returns the amount of inventory space used
    /// </summary>
    public int GetInventorySize => Inventory.inventory.Sum(loot => loot.amount);


    public int ConvertGold(int coins) => coins / 10000;
    public int ConvertSilver(int coins) => (coins % 10000) / 100;
    public int ConvertCopper(int coins) => coins % 100;

}
