using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image itemIcon;
    public Text itemNameText;

    private Item item;

    public void SetItem(Item newItem)
    {
        item = newItem;
        itemIcon.sprite = item.ItemIcon;
        itemNameText.text = item.ItemName;
    }

    public void OnClickUseItem()
    {
        Inventory.Instance.UseItem(item);
    }
}
