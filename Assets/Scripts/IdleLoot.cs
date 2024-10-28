using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IdleLoot : MonoBehaviour
{
    public DateTime lastCollected;

    [HideInInspector]
    public int maxIdleLoot;

    [HideInInspector]
    public int idleLootValue;

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
    
    void Start()
    {
        Saving.Instance.LoadIdleLoot();
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
        TimeSpan timeSinceLastCollected = DateTime.Now - lastCollected;
        int lootAmount = (int)timeSinceLastCollected.TotalSeconds / 60;
        lootAmount = Math.Clamp(lootAmount, 0, maxIdleLoot);
        return lootAmount;
    }
}
