using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Inventory : SingletonDontDestroyOnLoad<Inventory>
{
    private List<ItemSlot> invenItems = new List<ItemSlot>();
    public List<ItemSlot> items { get { return invenItems; } }

    private int invenSize = 21; // 인벤토리 크기

    public UnityAction onInventoryChanged; // 인벤토리가 변경되었을 때 호출되는 이벤트

    [Header("Inven UI")]
    public Transform slotParent; // 슬롯들이 배치될 부모 객체
    public GameObject slotUIPrefab; // 슬롯 UI 프리팹

    [SerializeField]
    public ItemDatabase itemDatabase; // 아이템 데이터베이스

    public Dictionary<int, int> ownItemsCount = new Dictionary<int, int>(); // 아이템 소유 수량

    private int curSelectSlot = -1; // 현재 선택된 슬롯 인덱스

    [Header("Inventory Info")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Image itemIcon;
    public TextMeshProUGUI itemStatText;
    public GameObject useButton;
    public GameObject dropButton;

    public PlayerController player; // 플레이어 컨트롤러

    public Transform dropPosition; // 아이템을 버릴 위치

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
        // 인벤토리 슬롯 초기화
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

        // 아이템 소유 수량 초기화
        foreach (var item in itemDatabase.ItemDatas)
        {
            ownItemsCount.Add(item.ItemID, 0);
        }
    }

    // 아이템 추가 메소드
    public void AddItem(Item item)
    {
        // 스택 가능한 아이템인지 확인하고 스택 가능하면 수량 증가
        if (item.ItemType == ItemType.Consumable && SearchStackSlot(item.ItemID, out int index))
        {
            ownItemsCount[item.ItemID] += 1;
            invenItems[index].AddItemCount();
            onInventoryChanged?.Invoke();
            return;
        }

        // 빈 슬롯이 있는지 확인하고 아이템 추가
        if (SearchEmptySlot(out index))
        {
            ownItemsCount[item.ItemID] += 1;
            invenItems[index].SetItem(item);
            onInventoryChanged?.Invoke();
            return;
        }

        // 인벤토리가 꽉 찼을 경우 아이템을 버림
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

    // 슬롯 비우기
    private void ClearInvenSlot(int index)
    {
        invenItems[index].ClearSlot();
    }

    // 아이템 삭제
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
            Debug.Log("잘못된 아이템 제거 시도");
        }
    }

    // 현재 선택된 슬롯의 아이템 삭제
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

    // 아이템 인덱스 검색
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

    // 아이템 버리기
    private void DropItem(Item item)
    {
        // 아이템 드롭 프리팹을 생성하여 현재 위치에 배치
        if (item.DropPrefab != null)
        {
            Instantiate(item.DropPrefab, dropPosition.position, Quaternion.identity);
            Debug.Log($"{item.ItemName}을(를) 버렸습니다.");
        }
        else
        {
            Debug.LogWarning("DropPrefab이 설정되지 않았습니다.");
        }
    }

    // 아이템 슬롯 선택
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

    // 아이템 정보창 초기화
    public void ClearInfo()
    {
        itemNameText.text = string.Empty;
        itemDescriptionText.text = string.Empty;
        itemStatText.text = string.Empty;
        itemIcon.enabled = false;
        useButton.SetActive(false);
        dropButton.SetActive(false);
    }

    // 아이템 정보창 설정
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

    // 아이템 사용 버튼 클릭
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

    // 인벤토리 UI 토글
    public void ToggleUI()
    {
        // 인벤토리 UI를 활성화/비활성화하고 커서 상태를 조정
        bool isActive = !slotParent.gameObject.activeSelf;
        slotParent.gameObject.SetActive(isActive);
        player.ToggleCursor(isActive);
    }

    // 아이템 수량 가져오기
    public int GetItemCount(Item item)
    {
        if (ownItemsCount.TryGetValue(item.ItemID, out int count))
        {
            return count;
        }
        return 0;
    }
}
