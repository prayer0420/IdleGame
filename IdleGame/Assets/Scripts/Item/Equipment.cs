using UnityEngine;
using UnityEngine.Events;

public class Equipment : MonoBehaviour
{
    private ItemSlot[] nowEquipItems = new ItemSlot[4];
    public ItemSlot[] NowEquipItems { get { return nowEquipItems; } }

    public Inventory inven;

    private int curSelectedIndex = -1;

    public GameObject equipButton;
    public GameObject unequipButton;

    public UnityAction onEquipChange;

    public GameObject currentEquippedWeapon;

    private void Awake()
    {
        inven = Inventory.Instance;

        for (int i = 0; i < nowEquipItems.Length; i++)
        {
            nowEquipItems[i] = new ItemSlot(inven);
            nowEquipItems[i].slotIndex = 100 + i;
        }
    }

    // 장비창에서 아이템 선택
    public void SelectItem(int index)
    {
        if (index < 0)
        {
            if (curSelectedIndex >= 0)
            {
                nowEquipItems[curSelectedIndex].SelectSlot(false);
            }
            curSelectedIndex = index;
            unequipButton.SetActive(false);
            return;
        }

        equipButton.SetActive(false);
        unequipButton.SetActive(true);

        if (curSelectedIndex >= 0)
        {
            nowEquipItems[curSelectedIndex].SelectSlot(false);
        }

        nowEquipItems[index].SelectSlot(true);
        inven.SetItemInfo(nowEquipItems[index].slotItem);
        curSelectedIndex = index;
    }

    // 무기 장착
    public void EquipWeapon(Item item)
    {
        Item temp = null;
        if (nowEquipItems[0].slotItem != null)
        {
            Destroy(currentEquippedWeapon);
            currentEquippedWeapon = null;
            temp = nowEquipItems[0].slotItem;
        }

        nowEquipItems[0].slotItem = item;
        inven.RemoveSelectSlotItem();

        if (temp != null)
        {
            inven.AddItem(temp);
        }

        // 무기 생성 및 장착
        if (item.EquipPrefab != null)
        {
            GameObject weapon = Instantiate(item.EquipPrefab, PlayerController.Instance.equipPosition);
            currentEquippedWeapon = weapon;
        }
        else
        {
            Debug.LogWarning("EquipPrefab이 설정되지 않았습니다.");
        }

        onEquipChange?.Invoke();
    }

    // 아이템 장비 해제
    public void UnEquipItem()
    {
        if (curSelectedIndex == 0)
        {
            Destroy(currentEquippedWeapon);
            currentEquippedWeapon = null;
        }

        Item temp = nowEquipItems[curSelectedIndex].slotItem;
        nowEquipItems[curSelectedIndex].ClearSlot();
        inven.AddItem(temp);
        inven.SelectItemSlot(-1);
        unequipButton.SetActive(false);
        onEquipChange?.Invoke();
    }
}
