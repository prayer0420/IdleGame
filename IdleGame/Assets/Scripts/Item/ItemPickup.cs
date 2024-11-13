using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ������ ȹ�� ���� (�ڵ����� ȹ��)
            Inventory.Instance.AddItem(item);
            UIManager.instance.ShowItemDescription(item);
            Destroy(gameObject);
        }
    }
}
