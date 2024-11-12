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
        upgradeButtons[0] = document.rootVisualElement.Q<VisualElement>("Upgrade1");
        upgradeButtons[1] = document.rootVisualElement.Q<VisualElement>("Upgrade2");
        upgradeButtons[2] = document.rootVisualElement.Q<VisualElement>("Upgrade3");
        upgradeButtons[3] = document.rootVisualElement.Q<VisualElement>("Upgrade4");
    }

    private void displayUpgrades()
    {
        DisplayCoins();
        for (int i = 0; i < upgradeButtons.Count(); i++)
        {
            VisualElement container = upgradeButtons[i];
            Upgrade upgrade = Saving.Instance.Upgrades.upgrades[i];
            container.Q<Label>("UpgradeName").text = upgrade.upgradeName;
            if(upgrade.upgradeName == "Sell Value")
            {
                container.Q<Label>("Current").text = upgrade.GetCurrentValue().ToString() + "x";
            }
            else
            {
                container.Q<Label>("Current").text = upgrade.GetCurrentValue().ToString();
            }
            if (upgrade.CanUpgrade())
            {
                container.Q<Label>("Added").text = "+" + upgrade.upgradeValue.ToString();
                container.Q<Label>("GoldText").text = PlayerInventory.Instance.ConvertGold(upgrade.GetUpgradeCost()).ToString();
                container.Q<Label>("SilverText").text = PlayerInventory.Instance.ConvertSilver(upgrade.GetUpgradeCost()).ToString();
                container.Q<Label>("CopperText").text = PlayerInventory.Instance.ConvertCopper(upgrade.GetUpgradeCost()).ToString();
            }
            else
            {
                container.Q<Label>("Added").text = "Max";
                container.Q<Label>("GoldText").text = "0";
                container.Q<Label>("SilverText").text = "0";
                container.Q<Label>("CopperText").text = "0";
            }
            int index = i;
            Button buyButton = container.Q<Button>("Buy");
            //Remove the previous event listener as this method can be called multiple times
            buyButton.UnregisterCallback<ClickEvent>(evt => PurchaseUpgrade(evt, index));
            buyButton.RegisterCallback<ClickEvent>(evt => PurchaseUpgrade(evt, index));
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
        salePrice += IdleLoot.Instance.SellIdleLoot();
        PlayerInventory.Instance.AddLoot(LootType.Copper, salePrice);
        Saving.Instance.Save();
    }

    public void PurchaseUpgrade(ClickEvent evt, int upgradeIndex)
    {
        int price = 0;
        Debug.Log(upgradeIndex);
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

    public void DisplayCoins()
    {
        VisualElement CoinDisplay = document.rootVisualElement.Q<VisualElement>("CoinDisplay");
        CoinDisplay.Q<Label>("GoldText").text = PlayerInventory.Instance.ConvertGold(PlayerInventory.Instance.Coins).ToString();
        CoinDisplay.Q<Label>("SilverText").text = PlayerInventory.Instance.ConvertSilver(PlayerInventory.Instance.Coins).ToString();
        CoinDisplay.Q<Label>("CopperText").text = PlayerInventory.Instance.ConvertCopper(PlayerInventory.Instance.Coins).ToString();
    }
}
