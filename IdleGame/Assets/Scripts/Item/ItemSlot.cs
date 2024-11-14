using UnityEngine;
using UnityEngine.Events;

public class ItemSlot
{
    public Item slotItem; // 슬롯에 있는 아이템
    public int slotIndex; // 슬롯 인덱스
    public int itemCount; // 아이템 수량
    public Inventory inven; // 인벤토리 참조
    public bool IsSelected { get; set; } = false; // 선택 여부

    public UnityAction onSelectChange; // 선택 변경 시 호출되는 이벤트

    public ItemSlot(Inventory slotInven)
    {
        inven = slotInven;
    }

    public void SetSlot(int index)
    {
        slotIndex = index;
    }

    public void SetItem(Item item)
    {
        slotItem = item;
        itemCount = 1;
        inven.onInventoryChanged?.Invoke();
    }

    public void AddItemCount()
    {
        itemCount++;
        inven.onInventoryChanged?.Invoke();
    }

    public void ClearSlot()
    {
        slotItem = null;
        itemCount = 0;
        SelectSlot(false);
    }

    public void SelectSlot(bool select)
    {
        IsSelected = select;
        onSelectChange?.Invoke();
    }
}
