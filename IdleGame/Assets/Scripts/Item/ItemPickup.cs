using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // æ∆¿Ã≈€ »πµÊ ∑Œ¡˜ (¿⁄µø¿∏∑Œ »πµÊ)
            Inventory.Instance.AddItem(item);
            UIManager.instance.ShowItemDescription(item);
            Destroy(gameObject);
        }
    }
}
