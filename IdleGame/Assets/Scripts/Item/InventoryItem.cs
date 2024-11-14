using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    public Text itemCountText;

    private ItemSlot itemSlot;

    public void SetupSlotUI(ItemSlot slot)
    {
        itemSlot = slot;
        itemSlot.onSelectChange += UpdateUI;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (itemSlot.slotItem != null)
        {
            itemIcon.sprite = itemSlot.slotItem.ItemIcon;
            itemIcon.enabled = true;
            if (itemSlot.itemCount > 1)
            {
                itemCountText.text = itemSlot.itemCount.ToString();
            }
            else
            {
                itemCountText.text = "";
            }
        }
        else
        {
            itemIcon.enabled = false;
            itemCountText.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Inventory.Instance.SelectItemSlot(itemSlot.slotIndex);
    }
}
