using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    public UIDocument document;
    private VisualElement background;
    private Button closeButton;
    private VisualElement[] upgradeButtons = new VisualElement[4];
    

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

    // Start is called before the first frame update
    void Start()
    {
        background = document.rootVisualElement.Q<VisualElement>("Background");
        closeButton = document.rootVisualElement.Q<Button>("CloseButton");
        closeButton.RegisterCallback<ClickEvent>(CloseButton);
        background.visible = false;
        for (int i = 0; i < upgradeButtons.Count(); i++)
        {
            upgradeButtons[i] = document.rootVisualElement.Q<VisualElement>("Upgrade" + i+1);
        }
    }

    private void displayUpgrades()
    {
        for (int i = 0; i < upgradeButtons.Count(); i++)
        {
            VisualElement container = upgradeButtons[i].Q<VisualElement>("Upgrade");
            Upgrade upgrade = Saving.Instance.Upgrades.upgrades[i];
            container.Q<Label>("UpgradeName").text = upgrade.upgradeName;
            container.Q<Label>("CurrentValue").text = upgrade.GetCurrentValue().ToString();
            if (upgrade.CanUpgrade())
            {
                container.Q<Label>("AddedValue").text = upgrade.upgradeValue.ToString();
                container.Q<Label>("GoldText").text = PlayerInventory.Instance.ConvertGold(upgrade.GetUpgradeCost()).ToString();
                container.Q<Label>("SilverText").text = PlayerInventory.Instance.ConvertSilver(upgrade.GetUpgradeCost()).ToString();
                container.Q<Label>("CopperText").text = PlayerInventory.Instance.ConvertCopper(upgrade.GetUpgradeCost()).ToString();
            }
            else
            {
                container.Q<Label>("AddedValue").text = "Max";
                container.Q<Label>("GoldText").text = "0";
                container.Q<Label>("SilverText").text = "0";
                container.Q<Label>("CopperText").text = "0";
            }
            container.Q<Button>("Buy").RegisterCallback<ClickEvent>(evt => PurchaseUpgrade(evt, i));
        }
    }

    public void CloseButton(ClickEvent evt)
    {
        Debug.Log("Closing Shop");
        background.visible = false;
    }

    public void OpenShop(ClickEvent evt)
    {
        Debug.Log("Opening Shop");
        displayUpgrades();
        background.visible = true;
    }

    public void SellItems()
    {
        //Sell items in the player's inventory
        int salePrice = (PlayerInventory.Instance.Inventory.inventory.Sum(loot => loot.amount * (int)loot.lootType)) * Saving.Instance.Upgrades.GetUpgrade(UpgradeType.SellValue).GetCurrentValue();
        PlayerInventory.Instance.ClearInventory();
        PlayerInventory.Instance.AddLoot(LootType.Copper, salePrice);
        IdleLoot.Instance.SellIdleLoot();
        MainUI.Instance.PrintInventory();
        Saving.Instance.Save();
    }

    public void PurchaseUpgrade(ClickEvent evt, int upgradeIndex)
    {
        int price = 0;
        Upgrade upgrade = Saving.Instance.Upgrades.upgrades[upgradeIndex];
        price = upgrade.GetUpgradeCost();
        if (PlayerInventory.Instance.PayWithCoins(price))
        {
            upgrade.UpgradeLevel();
            Saving.Instance.Save();
            displayUpgrades();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }
}
