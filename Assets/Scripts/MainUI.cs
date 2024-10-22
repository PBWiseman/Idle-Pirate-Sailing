using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance;

    private VisualElement document;
    private Label debugInventory;
    private Label debugAdded;
    private Label fpsCounter;

    private List<string> AddedLoot = new List<string>();

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
        document = GetComponent<UIDocument>().rootVisualElement;
    }

    // Start is called before the first frame update
    void Start()
    {
        debugInventory = document.Q<Label>("Debug_Inv");
        debugAdded = document.Q<Label>("Debug_Added");
        fpsCounter = document.Q<Label>("fps");
        PrintInventory();
    }

    void Update()
    {
        UpdateFps((int)(1f / Time.unscaledDeltaTime));
    }

    public void updateDisplay(LootType lootType, int amount)
    {
        //Update the display of the player's inventory
        AddedLoot.Add(lootType + ": " + amount);
        if (AddedLoot.Count > 3)
        {
            AddedLoot.RemoveAt(0);
        }
        debugAdded.text = "";
        foreach (string loot in AddedLoot)
        {
            debugAdded.text += loot + " ";
        }
        PrintInventory();
        
    }

    private void PrintInventory()
    {
        if (PlayerInventory.Instance.inventory.Count == 0)
        {
            debugInventory.text = "Inventory: Empty";
            return;
        }
        debugInventory.text = "Inventory " + PlayerInventory.Instance.CapacityRegular + ": ";
        foreach (LootAmount loot in PlayerInventory.Instance.inventory)
        {
            debugInventory.text += loot.lootType + ": " + loot.amount + " - ";
        }
        //remove the last " - " from the string
        debugInventory.text = debugInventory.text.Remove(debugInventory.text.Length - 3);
        debugInventory.text += "\n" + PlayerInventory.Instance.CoinDisplay;
    }

    public void UpdateFps(int fps)
    {
        fpsCounter.text = "FPS: " + fps;
    }
}
