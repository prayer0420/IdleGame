using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipment
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public int ItemID;
    public string ItemName;
    public ItemType ItemType;
    public string Description;
    public Sprite ItemIcon;

    public float AttackPower;
    public float DefensePower;
    public float EffectAmount;
    public float Duration;

    public GameObject DropPrefab; // 아이템을 드롭할 때 사용할 프리팹
    public GameObject EquipPrefab; // 장비 아이템의 경우 장착 시 사용할 프리팹
    public int MaxStackAmount = 99; // 아이템 최대 스택 수량

    // 아이템 사용 메소드
    public void UseItem()
    {
        IItem itemBehavior;

        if (ItemType == ItemType.Consumable)
        {
            itemBehavior = new ConsumableItem();
        }
        else if (ItemType == ItemType.Equipment)
        {
            itemBehavior = new EquipmentItem();
        }
        else
        {
            Debug.LogError("Unknown ItemType: " + ItemType);
            return;
        }

        itemBehavior.Use(this);
    }

    // 아이템 정보 텍스트 반환
    public string GetItemInfoText()
    {
        if (ItemType == ItemType.Consumable)
        {
            return $"회복량: {EffectAmount}";
        }
        else if (ItemType == ItemType.Equipment)
        {
            return $"공격력: {AttackPower}, 방어력: {DefensePower}";
        }
        else
        {
            return "알 수 없는 아이템 타입";
        }
    }
}
