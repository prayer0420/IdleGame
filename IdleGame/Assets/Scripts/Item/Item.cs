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

    public GameObject DropPrefab; // �������� ����� �� ����� ������
    public GameObject EquipPrefab; // ��� �������� ��� ���� �� ����� ������
    public int MaxStackAmount = 99; // ������ �ִ� ���� ����

    // ������ ��� �޼ҵ�
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

    // ������ ���� �ؽ�Ʈ ��ȯ
    public string GetItemInfoText()
    {
        if (ItemType == ItemType.Consumable)
        {
            return $"ȸ����: {EffectAmount}";
        }
        else if (ItemType == ItemType.Equipment)
        {
            return $"���ݷ�: {AttackPower}, ����: {DefensePower}";
        }
        else
        {
            return "�� �� ���� ������ Ÿ��";
        }
    }
}
