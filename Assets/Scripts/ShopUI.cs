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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseButton(ClickEvent evt)
    {
        Debug.Log("Closing Shop");
        background.visible = false;
    }

    public void OpenShop(ClickEvent evt)
    {
        Debug.Log("Opening Shop");
        background.visible = true;
    }

    public void SellItems()
    {
        //Sell items in the player's inventory
        int salePrice = PlayerInventory.Instance.Inventory.inventory.Sum(loot => loot.amount * (int)loot.lootType);
        PlayerInventory.Instance.ClearInventory();
        PlayerInventory.Instance.AddLoot(LootType.Copper, salePrice);
        IdleLoot.Instance.SellIdleLoot();
        MainUI.Instance.PrintInventory();
        Saving.Instance.Save();
    }

    public void PurchaseUpgrade()
    {
        int price = 0;
        //Code for buying the upgrade
        //Check if the player has enough coins
        if (PlayerInventory.Instance.PayWithCoins(price))
        {
            //Success
        }
        else
        {
            //Failure
        }
    }
}
