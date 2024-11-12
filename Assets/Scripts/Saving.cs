using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class Saving : MonoBehaviour
{
    public static Saving Instance;
    private float nextSaveTime = 60f;
    private SaveData saveData;
    public Inventory Inventory
    {
        get => saveData.inventory;
        set => saveData.inventory = value;
    }
    public Upgrades Upgrades
    {
        get => saveData.upgrades;
        set => saveData.upgrades = value;
    }
    public DateTime LastCollected
    {
        get
        {
            try
            {
                return System.DateTime.Parse(saveData.lastCollected);
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Failed to parse last collected date, returning current date - " + saveData.lastCollected);
                return System.DateTime.Now;
            }
        }
        set
        {
            saveData.lastCollected = value.ToString();
        }
    }

    //Persistent data path
    private string savePath => $"{Application.persistentDataPath}/inventory.json";

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
        Load();
    }

    // Update is called once per frame
    void Update()
    {
        //Save every 60 seconds
        if (Time.time >= nextSaveTime)
        {
            Save();
            nextSaveTime = Time.time + 60f;
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
        else
        {
            MainUI.Instance.PrintInventory();
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
        }
        else
        {
            MainUI.Instance.PrintInventory();
        }
    }

    public bool Save()
    {
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save file: {e}");
            return false;
        }
    }

    public void Load()
    {
        try
        {
            string json;
            if (!File.Exists(savePath))
            {
                //Create a new save file
                saveData = new SaveData();
                saveData.inventory = new Inventory();
                saveData.upgrades = defaultUpgrades();                
                saveData.lastCollected = System.DateTime.Now.ToString();
                json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(savePath, json);
            }
            else
            {
                json = File.ReadAllText(savePath);
                saveData = JsonUtility.FromJson<SaveData>(json);
                if (saveData.upgrades == null)
                {
                    saveData.upgrades = defaultUpgrades();
                }
                if (saveData.lastCollected == null)
                {
                    saveData.lastCollected = System.DateTime.Now.ToString();
                }
                if (saveData.inventory == null)
                {
                    saveData.inventory = new Inventory();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load file: {e}");
            saveData = new SaveData();
        }
    }

    private Upgrades defaultUpgrades()
    {
        Upgrades upgrades = new Upgrades();
        upgrades.AddUpgrade(UpgradeType.InventorySize, 1, -1, 50, 10, 50);
        upgrades.AddUpgrade(UpgradeType.IdleLootValue, 1, -1, 100, 1, 1);
        upgrades.AddUpgrade(UpgradeType.MaxIdleLoot, 1, -1, 50, 10, 100);
        upgrades.AddUpgrade(UpgradeType.SellValue, 1, -1, 1000, 1, 1);
        return upgrades;
    }
}
