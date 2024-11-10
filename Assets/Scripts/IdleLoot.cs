using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IdleLoot : MonoBehaviour
{
    DateTime lastCollected
    {
        get => Saving.Instance.LastCollected;
        set => Saving.Instance.LastCollected = value;
    }

    int maxIdleLoot
    {
        get
        {
            Upgrade upgrade = Saving.Instance.Upgrades.GetUpgrade(UpgradeType.MaxIdleLoot);
            return upgrade.GetCurrentValue();
        }
    }

    int idleLootValue
    {
        get
        {
            Upgrade upgrade = Saving.Instance.Upgrades.GetUpgrade(UpgradeType.IdleLootValue);
            return upgrade.GetCurrentValue();
        }
    }

    public static IdleLoot Instance;
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

    public void SellIdleLoot()
    {
        int lootAmount = availableLoot();
        if (lootAmount == 0)
        {
            return;
        }
        PlayerInventory.Instance.AddLoot(LootType.Copper, lootAmount * idleLootValue);
        resetIdleLoot();
    }

    public void resetIdleLoot()
    {
        lastCollected = DateTime.Now;
        Saving.Instance.Save();
    }

    public int availableLoot()
    {
        try
        {
            TimeSpan timeSinceLastCollected = DateTime.Now - lastCollected;
            int lootAmount = (int)timeSinceLastCollected.TotalSeconds / 60;
            lootAmount = Math.Clamp(lootAmount, 0, maxIdleLoot);
            return lootAmount;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return 0;
        }
    }
}
