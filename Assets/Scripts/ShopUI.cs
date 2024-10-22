using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SellItems()
    {
        //Sell items in the player's inventory
        int salePrice = 0;
        foreach (LootAmount loot in PlayerInventory.Instance.inventory)
        {
            salePrice += loot.amount * (int)loot.lootType;
            PlayerInventory.Instance.RemoveLoot(loot.lootType, loot.amount);
        }
        PlayerInventory.Instance.AddLoot(LootType.Copper, salePrice);
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
