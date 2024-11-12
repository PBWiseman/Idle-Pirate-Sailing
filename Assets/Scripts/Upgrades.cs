using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum UpgradeType
{
    InventorySize,
    IdleLootValue,
    MaxIdleLoot,
    SellValue
}

public class UpgradeDictionary
{
    public static Dictionary<UpgradeType, string> upgradeNames = new Dictionary<UpgradeType, string>
    {
        { UpgradeType.InventorySize, "Inventory Size" },
        { UpgradeType.IdleLootValue, "Idle Loot Value" },
        { UpgradeType.MaxIdleLoot, "Max Idle Loot" },
        { UpgradeType.SellValue, "Sell Value" }
    };
}

[Serializable]
public class Upgrade
{
    public UpgradeType upgradeType;
    public string upgradeName => UpgradeDictionary.upgradeNames[upgradeType];
    public int currentLevel;
    public int maxLevel; //If maxLevel is -1 upgrade is uncapped
    public int upgradeCost;
    public int upgradeValue;
    public int baseValue;

    public Upgrade(UpgradeType upgradeType, int currentLevel, int maxLevel, int upgradeCost, int upgradeValue, int baseValue)
    {
        this.upgradeType = upgradeType;
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
        this.upgradeCost = upgradeCost;
        this.upgradeValue = upgradeValue;
        this.baseValue = baseValue;
    }

    public int GetCurrentValue()
    {
        return baseValue + (upgradeValue * (currentLevel-1));
    }

    public int GetUpgradeCost()
    {
        return upgradeCost * currentLevel;
    }
    
    public bool UpgradeLevel()
    {
        if (CanUpgrade())
        {
            currentLevel++;
            return true;
        }
        return false;
    }

    public bool CanUpgrade()
    {
        return currentLevel < maxLevel || maxLevel == -1;
    }
}

[Serializable]
public class Upgrades
{
    public List<Upgrade> upgrades = new List<Upgrade>();

    public void AddUpgrade(UpgradeType upgradeType, int currentLevel, int maxLevel, int upgradeCost, int upgradeValue, int baseValue)
    {
        upgrades.Add(new Upgrade(upgradeType, currentLevel, maxLevel, upgradeCost, upgradeValue, baseValue));
    }

    public Upgrade GetUpgrade(UpgradeType upgradeType)
    {
        return upgrades.Find(upgrade => upgrade.upgradeType == upgradeType);
    }
}

[Serializable]
public class SaveData
{
    public Inventory inventory;
    public Upgrades upgrades;
    public string lastCollected;
}