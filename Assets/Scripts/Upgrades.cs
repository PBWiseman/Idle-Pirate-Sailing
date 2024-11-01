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

[Serializable]
public class Upgrade
{
    public UpgradeType upgradeType;
    public int currentLevel;
    public int maxLevel; //If maxLevel is -1 upgrade is uncapped
    public int upgradeCost;
    public int upgradeValue;

    public Upgrade(UpgradeType upgradeType, int currentLevel, int maxLevel, int upgradeCost, int upgradeValue)
    {
        this.upgradeType = upgradeType;
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
        this.upgradeCost = upgradeCost;
        this.upgradeValue = upgradeValue;
    }

    public int GetCurrentValue()
    {
        return upgradeValue * currentLevel;
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

    public void ConvertSaveData(UpgradeSaveData upgradeSaveData)
    {
        Upgrade upgrade = GetUpgrade(upgradeSaveData.upgradeType);
        upgrade.currentLevel = upgradeSaveData.currentLevel;
    }

    public void AddUpgrade(UpgradeType upgradeType, int currentLevel, int maxLevel, int upgradeCost, int upgradeValue)
    {
        upgrades.Add(new Upgrade(upgradeType, currentLevel, maxLevel, upgradeCost, upgradeValue));
    }

    public Upgrade GetUpgrade(UpgradeType upgradeType)
    {
        return upgrades.Find(upgrade => upgrade.upgradeType == upgradeType);
    }
}

public class UpgradeSaveData
{
    public UpgradeType upgradeType;
    public int currentLevel;

    public UpgradeSaveData(List<Upgrade> upgrades)
    {
        foreach (Upgrade upgrade in upgrades)
        {
            upgradeType = upgrade.upgradeType;
            currentLevel = upgrade.currentLevel;
        }
    }
}

[Serializable]
public class SaveData
{
    public Inventory inventory;
    public UpgradeSaveData upgrades;
    public int lastCollected;
}