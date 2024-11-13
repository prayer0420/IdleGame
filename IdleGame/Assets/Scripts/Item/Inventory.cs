using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : SingletonDontDestroyOnLoad<Inventory>
{

    public List<Item> items = new List<Item>();

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        UIManager.Instance.UpdateInventoryUI();
    }

    public void UseItem(Item item)
    {
        IItem useItem = null;

        if (item.ItemType == ItemType.Consumable)
        {
            useItem = new ConsumableItem();
        }
        else if (item.ItemType == ItemType.Equipment)
        {
            useItem = new EquipmentItem();
        }

        useItem?.Use(item);

        if (item.ItemType == ItemType.Consumable)
        {
            // 소비 아이템은 사용 후 제거
            items.Remove(item);
            UIManager.Instance.UpdateInventoryUI();
        }
    }

}
