using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Inventory : SingletonDontDestroyOnLoad<Inventory>
{
    private List<ItemSlot> invenItems = new List<ItemSlot>();
    public List<ItemSlot> items { get { return invenItems; } }

    private int invenSize = 21; // �κ��丮 ũ��

    public UnityAction onInventoryChanged; // �κ��丮�� ����Ǿ��� �� ȣ��Ǵ� �̺�Ʈ

    [Header("Inven UI")]
    public Transform slotParent; // ���Ե��� ��ġ�� �θ� ��ü
    public GameObject slotUIPrefab; // ���� UI ������

    [SerializeField]
    public ItemDatabase itemDatabase; // ������ �����ͺ��̽�

    public Dictionary<int, int> ownItemsCount = new Dictionary<int, int>(); // ������ ���� ����

    private int curSelectSlot = -1; // ���� ���õ� ���� �ε���

    [Header("Inventory Info")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Image itemIcon;
    public TextMeshProUGUI itemStatText;
    public GameObject useButton;
    public GameObject dropButton;

    public PlayerController player; // �÷��̾� ��Ʈ�ѷ�

    public Transform dropPosition; // �������� ���� ��ġ

    protected override void Awake()
    {
        base.Awake();

    }

    public void Start()
    {

        player = PlayerController.Instance;

        ClearInfo();
    }


    public void InitInventory()
    {
        // �κ��丮 ���� �ʱ�ȭ
        for (int i = 0; i < invenSize; i++)
        {
            invenItems.Add(new ItemSlot(this));
            invenItems[i].SetSlot(i);

            GameObject slotUI = Instantiate(slotUIPrefab, slotParent);
            SlotUI slotUIScript = slotUI.GetComponent<SlotUI>();
            if (slotUIScript != null)
            {
                slotUIScript.SetupSlotUI(invenItems[i]);
            }
        }

        // ������ ���� ���� �ʱ�ȭ
        foreach (var item in itemDatabase.ItemDatas)
        {
            ownItemsCount.Add(item.ItemID, 0);
        }
    }

    // ������ �߰� �޼ҵ�
    public void AddItem(Item item)
    {
        // ���� ������ ���������� Ȯ���ϰ� ���� �����ϸ� ���� ����
        if (item.ItemType == ItemType.Consumable && SearchStackSlot(item.ItemID, out int index))
        {
            ownItemsCount[item.ItemID] += 1;
            invenItems[index].AddItemCount();
            onInventoryChanged?.Invoke();
            return;
        }

        // �� ������ �ִ��� Ȯ���ϰ� ������ �߰�
        if (SearchEmptySlot(out index))
        {
            ownItemsCount[item.ItemID] += 1;
            invenItems[index].SetItem(item);
            onInventoryChanged?.Invoke();
            return;
        }

        // �κ��丮�� �� á�� ��� �������� ����
        DropItem(item);
    }

    private bool SearchEmptySlot(out int index)
    {
        for (int i = 0; i < invenSize; i++)
        {
            if (invenItems[i].slotItem == null)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private bool SearchStackSlot(int itemId, out int index)
    {
        for (int i = 0; i < invenSize; i++)
        {
            if (invenItems[i].slotItem == null) continue;

            if (invenItems[i].slotItem.ItemID == itemId && invenItems[i].slotItem.MaxStackAmount > invenItems[i].itemCount)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    // ���� ����
    private void ClearInvenSlot(int index)
    {
        invenItems[index].ClearSlot();
    }

    // ������ ����
    public void RemoveItem(Item item)
    {
        int index = SearchItemIndexInInven(item);

        if (index >= 0)
        {
            ownItemsCount[item.ItemID] -= 1;
            invenItems[index].itemCount -= 1;

            if (invenItems[index].itemCount <= 0)
            {
                ClearInvenSlot(index);
                ClearInfo();
            }
            onInventoryChanged?.Invoke();
        }
        else
        {
            Debug.Log("�߸��� ������ ���� �õ�");
        }
    }

    // ���� ���õ� ������ ������ ����
    public void RemoveSelectSlotItem()
    {
        if (curSelectSlot < 0) return;

        invenItems[curSelectSlot].itemCount -= 1;
        if (invenItems[curSelectSlot].itemCount <= 0)
        {
            ClearInvenSlot(curSelectSlot);
            SelectItemSlot(-1);
            ClearInfo();
        }
        onInventoryChanged?.Invoke();
    }

    // ������ �ε��� �˻�
    private int SearchItemIndexInInven(Item item)
    {
        for (int i = 0; i < invenSize; i++)
        {
            if (invenItems[i].slotItem == item)
            {
                return i;
            }
        }

        return -1;
    }

    // ������ ������
    private void DropItem(Item item)
    {
        // ������ ��� �������� �����Ͽ� ���� ��ġ�� ��ġ
        if (item.DropPrefab != null)
        {
            Instantiate(item.DropPrefab, dropPosition.position, Quaternion.identity);
            Debug.Log($"{item.ItemName}��(��) ���Ƚ��ϴ�.");
        }
        else
        {
            Debug.LogWarning("DropPrefab�� �������� �ʾҽ��ϴ�.");
        }
    }

    // ������ ���� ����
    public void SelectItemSlot(int index)
    {
        if (index < 0)
        {
            if (curSelectSlot >= 0)
            {
                invenItems[curSelectSlot].SelectSlot(false);
            }
            ClearInfo();
            curSelectSlot = index;
            return;
        }

        if (curSelectSlot >= 0)
        {
            invenItems[curSelectSlot].SelectSlot(false);
        }

        curSelectSlot = index;
        invenItems[index].SelectSlot(true);
        SetItemInfo(invenItems[index].slotItem);
    }

    // ������ ����â �ʱ�ȭ
    public void ClearInfo()
    {
        itemNameText.text = string.Empty;
        itemDescriptionText.text = string.Empty;
        itemStatText.text = string.Empty;
        itemIcon.enabled = false;
        useButton.SetActive(false);
        dropButton.SetActive(false);
    }

    // ������ ����â ����
    public void SetItemInfo(Item item)
    {
        itemNameText.text = item.ItemName;
        itemDescriptionText.text = item.Description;
        itemIcon.enabled = true;
        itemIcon.sprite = item.ItemIcon;

        itemStatText.text = item.GetItemInfoText();

        useButton.SetActive(true);
        dropButton.SetActive(true);
    }

    // ������ ��� ��ư Ŭ��
    public void OnUseButtonClick()
    {
        if (curSelectSlot < 0) return;

        invenItems[curSelectSlot].slotItem.UseItem();
        RemoveSelectSlotItem();
    }

    public void OnDropButtonClick()
    {
        if (curSelectSlot < 0) return;

        DropItem(invenItems[curSelectSlot].slotItem);
        RemoveSelectSlotItem();
    }

    // �κ��丮 UI ���
    public void ToggleUI()
    {
        // �κ��丮 UI�� Ȱ��ȭ/��Ȱ��ȭ�ϰ� Ŀ�� ���¸� ����
        bool isActive = !slotParent.gameObject.activeSelf;
        slotParent.gameObject.SetActive(isActive);
        player.ToggleCursor(isActive);
    }

    // ������ ���� ��������
    public int GetItemCount(Item item)
    {
        if (ownItemsCount.TryGetValue(item.ItemID, out int count))
        {
            return count;
        }
        return 0;
    }
}
