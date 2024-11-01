using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Saving : MonoBehaviour
{
    public static Saving Instance;
    private float nextSaveTime = 60f;

    private const string IDLE_LAST_COLLECTED = "LastCollected";
    private const string IDLE_LOOT_VALUE = "IdleLootValue";
    private const string MAX_IDLE_LOOT = "MaxIdleLoot";

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
            string json = JsonUtility.ToJson(PlayerInventory.Instance.Inventory, true);
            File.WriteAllText(savePath, json);
            PlayerPrefs.SetString(IDLE_LAST_COLLECTED, IdleLoot.Instance.lastCollected.ToString());
            PlayerPrefs.SetInt(IDLE_LOOT_VALUE, IdleLoot.Instance.idleLootValue);
            PlayerPrefs.SetInt(MAX_IDLE_LOOT, IdleLoot.Instance.maxIdleLoot);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save file: {e}");
            return false;
        }
    }

    public Inventory Load()
    {
        try
        {
            string json;
            if (!File.Exists(savePath))
            {
                json = JsonUtility.ToJson(new Inventory(), true);
                File.WriteAllText(savePath, json);
            }
            json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<Inventory>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load file: {e}");
            return null;
        }
    }

    public void LoadIdleLoot()
    {
        IdleLoot.Instance.idleLootValue = PlayerPrefs.GetInt(IDLE_LOOT_VALUE, 1);
        IdleLoot.Instance.maxIdleLoot = PlayerPrefs.GetInt(MAX_IDLE_LOOT, 100);
        IdleLoot.Instance.lastCollected = System.DateTime.Parse(PlayerPrefs.GetString(IDLE_LAST_COLLECTED, System.DateTime.Now.ToString()));
    }
}
