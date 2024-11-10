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
    private VisualElement[] upgradesButtons = new VisualElement[4];
    

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
        for (int i = 0; i < upgrades.Count(); i++)
        {
            upgrades[i] = document.rootVisualElement.Q<VisualElement>("Upgrade" + i+1);
        }
    }

    private void displayUpgrades()
    {
        for (int i = 0; i < upgradesButtons.Count(); i++)
        {
            Upgrade upgrade = Saving.Instance.Upgrades.upgrades[i];
            upgrades[i].Q<Label>("UpgradeName").text = upgrade.upgradeName;
            upgrades[i].Q<Label>("CurrentValue").text = upgrade.GetCurrentValue();
            if (upgrade.CanUpgrade())
            {
                upgrades[i].Q<Label>("AddedValue").text = upgrade.upgradeValue;
                upgrades[i].Q<Label>("GoldText").text = PlayerInventory.Instance.ConvertGold(upgrade.GetUpgradeCost());
                upgrades[i].Q<Label>("SilverText").text = PlayerInventory.Instance.ConvertSilver(upgrade.GetUpgradeCost());
                upgrades[i].Q<Label>("CopperText").text = PlayerInventory.Instance.ConvertCopper(upgrade.GetUpgradeCost());
            }
            else
            {
                upgrades[i].Q<Label>("AddedValue").text = "Max";
                upgrades[i].Q<Label>("GoldText").text = "0";
                upgrades[i].Q<Label>("SilverText").text = "0";
                upgrades[i].Q<Label>("CopperText").text = "0";
            }
            upgrades[i].Q<Button>("Buy").RegisterCallback<ClickEvent>(evt => PurchaseUpgrade(evt, i));
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
