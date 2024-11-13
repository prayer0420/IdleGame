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
}
