using System;

[Serializable]
public class ItemData
{
    public int ItemID;
    public string ItemName;
    public string ItemType;
    public string Description;
    public float AttackPower;
    public float DefensePower;
    public float EffectAmount;
}

[Serializable]
public class EnemyData
{
    public int EnemyID;
    public string EnemyName;
    public float Health;
    public float AttackPower;
    public float DefensePower;
}


[Serializable]
public class CharacterData
{
    public float Health;
    public float AttackPower;
    public float DefensePower;
}