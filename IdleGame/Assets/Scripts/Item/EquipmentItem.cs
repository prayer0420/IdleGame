using UnityEngine;

public class EquipmentItem: IItem
{
    public void Use(Item item)
    {
        PlayerController player = PlayerController.Instance;

        if (item.ItemName.Contains("Sword"))
        {
            // 이전 무기 해제
            if (player.EquippedWeapon != null)
            {
                player.AttackPower -= player.EquippedWeapon.AttackPower;
            }

            // 새로운 무기 장착
            player.EquippedWeapon = item;
            player.AttackPower += item.AttackPower;
        }
        else if (item.ItemName.Contains("Armor"))
        {
            // 이전 방어구 해제
            if (player.EquippedArmor != null)
            {
                player.DefensePower -= player.EquippedArmor.DefensePower;
            }

            // 새로운 방어구 장착
            player.EquippedArmor = item;
            player.DefensePower += item.DefensePower;
        }

    }
}
