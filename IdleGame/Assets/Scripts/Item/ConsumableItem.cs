using UnityEngine;

public class ConsumableItem: IItem
{
    public void Use(Item item)
    {
        PlayerController player = PlayerController.Instance;

        player.Health += item.EffectAmount;

        // ü�� �ִ�ġ ����
        player.Health = Mathf.Min(player.Health, player.MaxHealth);

        // UI ������Ʈ
        UIManager.Instance.UpdateHPBar(player.Health / player.MaxHealth);
    }
}
