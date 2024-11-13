using UnityEngine;

public class ConsumableItem: IItem
{
    public void Use(Item item)
    {
        PlayerController player = PlayerController.Instance;

        player.Health += item.EffectAmount;

        // 체력 최대치 제한
        player.Health = Mathf.Min(player.Health, player.MaxHealth);

        // UI 업데이트
        UIManager.Instance.UpdateHPBar(player.Health / player.MaxHealth);
    }
}
