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
    private Button shopButton;

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
        shopButton = document.Q<Button>("ShopButton");
        shopButton.RegisterCallback<ClickEvent>(ShopUI.Instance.OpenShop);
        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(.2f);
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
        for (int i = AddedLoot.Count - 1; i >= 0; i--)
        {
            debugAdded.text += AddedLoot[i] + " ";
        }
        PrintInventory();
    }

    public void PrintInventory()
    {
        try
        {
            if (PlayerInventory.Instance.Inventory.inventory == null || PlayerInventory.Instance.Inventory.inventory.Count == 0)
            {
                if (debugInventory != null)
                {
                    debugInventory.text = "Inventory: Empty";
                }
            }
            else
            {
                debugInventory.text = "Inventory " + PlayerInventory.Instance.CapacityRegular + ": ";
                foreach (LootAmount loot in PlayerInventory.Instance.Inventory.inventory)
                {
                    debugInventory.text += loot.lootType + ": " + loot.amount + " - ";
                }
                //remove the last " - " from the string
                debugInventory.text = debugInventory.text.Remove(debugInventory.text.Length - 3);
            }
            if (debugInventory != null)
            {
                debugInventory.text += "\n" + PlayerInventory.Instance.CoinDisplay;
                debugInventory.text += "\n" + "Idle Loot: " + IdleLoot.Instance.availableLoot() + "/" + Saving.Instance.Upgrades.GetUpgrade(UpgradeType.MaxIdleLoot).GetCurrentValue();
            }
        }
        catch
        {
            Debug.Log("Not loaded");
        }
    }

    public void UpdateFps(int fps)
    {
        fpsCounter.text = "FPS: " + fps;
    }
}
