using UnityEngine;

public class EquipmentItem: IItem
{
    public void Use(Item item)
    {
        PlayerController player = PlayerController.Instance;

        if (item.ItemName.Contains("Sword"))
        {
            // ���� ���� ����
            if (player.EquippedWeapon != null)
            {
                player.AttackPower -= player.EquippedWeapon.AttackPower;
            }

            // ���ο� ���� ����
            player.EquippedWeapon = item;
            player.AttackPower += item.AttackPower;
        }
        else if (item.ItemName.Contains("Armor"))
        {
            // ���� �� ����
            if (player.EquippedArmor != null)
            {
                player.DefensePower -= player.EquippedArmor.DefensePower;
            }

            // ���ο� �� ����
            player.EquippedArmor = item;
            player.DefensePower += item.DefensePower;
        }

    }
}
