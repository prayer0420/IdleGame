using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonDontDestroyOnLoad<UIManager>
{
    public static UIManager instance;


    public Image hpBar;
    public TextMeshProUGUI goldText;
    public GameObject inventoryPanel;
    public Transform inventoryContent;
    public GameObject inventoryItemPrefab;

    public TextMeshProUGUI itemDescriptionText;

    //ü��UI ������Ʈ
    private IEnumerator SmoothFill(float targetRatio, float duration)
    {
        float startRatio = hpBar.fillAmount;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hpBar.fillAmount = Mathf.Lerp(startRatio, targetRatio, elapsed / duration);
            yield return null;
        }

        hpBar.fillAmount = targetRatio;
    }

    public void UpdateHPBar(float hpRatio)
    {
        StartCoroutine(SmoothFill(hpRatio, 0.5f)); 
    }

    //���UI ������Ʈ
    public void UpdateGoldText(int amount)
    {
        goldText.text = amount.ToString();
    }

    //�κ��丮UI ������Ʈ
    public void UpdateInventoryUI()
    {
        // ���� ������ UI ����
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        // ���ο� ������ UI ����
        foreach (var item in Inventory.Instance.items)
        {
            GameObject itemObj = Instantiate(inventoryItemPrefab, inventoryContent);
            InventoryItem inventoryItem = itemObj.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                inventoryItem.SetupSlotUI(item);
            }
        }
    }

    public void ToggleInventoryPanel()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    public void ShowItemDescription(Item item)
    {
        itemDescriptionText.text = item.Description;

        // ������ ���� UI�� Ȱ��ȭ
        itemDescriptionText.gameObject.SetActive(true);

        // ���� �ð� �� ��Ȱ��ȭ
        Invoke("HideItemDescription", 2f); // 2�� �� ���� �����
    }

    public void HideItemDescription()
    {
        itemDescriptionText.gameObject.SetActive(false);
    }
}
