using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance;

    private VisualElement document;
    private Button shopButton;
    public VisualElement OuterContainer;
    private List<string> AddedLoot = new List<string>();

    private class LootDisplay
    {
        public Label label;
        public LootType lootType;
        public int amount;
        private float lastUpdated = 0;
        private int lastAmountAdded = 0;
        private int capacity = 0;

        public LootDisplay(VisualElement OuterContainer, string labelName, LootType lootType, int amount, int capacity = 0)
        {
            label = OuterContainer.Q<Label>(labelName);
            this.lootType = lootType;
            this.amount = amount;
            label.text = amount.ToString();
        }

        public void SetAmount(int amount)
        {
            this.amount = amount;
            UpdateDisplay();
        }

        public void SetCapacity(int capacity)
        {
            this.capacity = capacity;
            UpdateDisplay();
        }

        public void AddAmount(int added)
        {
            lastUpdated = Time.time;
            lastAmountAdded = added;
        }

        public void UpdateDisplay()
        {
            label.text = amount.ToString();
            if (capacity > 0)
            {
                label.text += "/" + capacity.ToString();
            }
            if (amount > 0 && Time.time - lastUpdated < 2 && lastAmountAdded > 0) //If the amount is greater than 0 and the last update was less than 2 seconds ago
            {
                label.text += " +" + lastAmountAdded;
            }
        }
    }

    private List<LootDisplay> lootDisplays = new List<LootDisplay>();
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
        OuterContainer = document.Q<VisualElement>("OuterContainer");
        lootDisplays.Add(new LootDisplay(OuterContainer, "IdleCount", LootType.IdleLoot, 0, 0));
        lootDisplays.Add(new LootDisplay(OuterContainer, "InventoryCount", LootType.Inventory, 0, 0));
        lootDisplays.Add(new LootDisplay(OuterContainer, "FabricCount", LootType.Fabric, 0));
        lootDisplays.Add(new LootDisplay(OuterContainer, "WoodCount", LootType.Wood, 0));
        lootDisplays.Add(new LootDisplay(OuterContainer, "MetalCount", LootType.Metal, 0));
        lootDisplays.Add(new LootDisplay(OuterContainer, "CopperCount", LootType.Copper, 0));
        lootDisplays.Add(new LootDisplay(OuterContainer, "SilverCount", LootType.Silver, 0));
        lootDisplays.Add(new LootDisplay(OuterContainer, "GoldCount", LootType.Gold, 0));
        shopButton = document.Q<Button>("ShopButton");
        shopButton.RegisterCallback<ClickEvent>(ShopUI.Instance.OpenShop);
        StartCoroutine(StartPrintInventory());
    }

    //Call print inventory every .2 seconds until it returns true
    IEnumerator StartPrintInventory()
    {
        while (!PrintInventory())
        {
            yield return new WaitForSeconds(.2f);
        }
        StartCoroutine(UpdateInventory());
    }

    IEnumerator UpdateInventory()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            PrintInventory();
        }
    }

    public void updateDisplay(LootType lootType, int amount)
    {
        foreach (LootDisplay lootDisplay in lootDisplays)
        {
            if (lootDisplay.lootType == lootType)
            {
                lootDisplay.AddAmount(amount);
            }
        }
        PrintInventory();
    }

    public bool PrintInventory()
    {
        try
        {
            foreach (LootDisplay lootDisplay in lootDisplays)
            {
                switch (lootDisplay.lootType)
                {
                    case LootType.IdleLoot:
                        lootDisplay.SetAmount(IdleLoot.Instance.availableLoot());
                        lootDisplay.SetCapacity(IdleLoot.Instance.maxIdleLoot);
                        break;
                    case LootType.Inventory:
                        lootDisplay.SetAmount(PlayerInventory.Instance.GetInventorySize);
                        lootDisplay.SetCapacity(PlayerInventory.Instance.maxInventorySize);
                        break;
                    case LootType.Fabric:
                        lootDisplay.SetAmount(PlayerInventory.Instance.GetLootAmount(LootType.Fabric));
                        break;
                    case LootType.Wood:
                        lootDisplay.SetAmount(PlayerInventory.Instance.GetLootAmount(LootType.Wood));
                        break;
                    case LootType.Metal:
                        lootDisplay.SetAmount(PlayerInventory.Instance.GetLootAmount(LootType.Metal));
                        break;
                    case LootType.Copper:
                        lootDisplay.SetAmount(PlayerInventory.Instance.ConvertCopper(PlayerInventory.Instance.Coins));
                        break;
                    case LootType.Silver:
                        lootDisplay.SetAmount(PlayerInventory.Instance.ConvertSilver(PlayerInventory.Instance.Coins));
                        break;
                    case LootType.Gold:
                        lootDisplay.SetAmount(PlayerInventory.Instance.ConvertGold(PlayerInventory.Instance.Coins));
                        break;
                }
            }
            return true;
        }
        catch
        {
            Debug.Log("Not loaded");
            return false;
        }
    }
}
