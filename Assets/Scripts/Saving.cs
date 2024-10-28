using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Saving : MonoBehaviour
{
    public static Saving Instance;
    private float nextSaveTime = 60f;

    //Persistent data path
    private string savePath => $"{Application.persistentDataPath}/highscores.json";

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
            Save(PlayerInventory.Instance.inventory);
            nextSaveTime = Time.time + 60f;
        }
    }

    void OnApplicationQuit()
    {
        Save(PlayerInventory.Instance.inventory);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save(PlayerInventory.Instance.inventory);
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save(PlayerInventory.Instance.inventory);
        }
    }

    public bool Save(List<LootAmount> inventory)
    {
        try
        {
            string json = JsonUtility.ToJson(inventory, true);
            File.WriteAllText(savePath, json);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save file: {e}");
            return false;
        }
    }

    public List<LootAmount> Load()
    {
        try
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<List<LootAmount>>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load file: {e}");
            return null;
        }
    }
}
