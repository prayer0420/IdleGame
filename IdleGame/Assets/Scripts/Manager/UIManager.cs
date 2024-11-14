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

    //체력UI 업데이트
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

    //골드UI 업데이트
    public void UpdateGoldText(int amount)
    {
        goldText.text = amount.ToString();
    }

    //인벤토리UI 업데이트
    public void UpdateInventoryUI()
    {
        // 기존 아이템 UI 삭제
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 아이템 UI 생성
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

        // 아이템 설명 UI를 활성화
        itemDescriptionText.gameObject.SetActive(true);

        // 일정 시간 후 비활성화
        Invoke("HideItemDescription", 2f); // 2초 후 설명 숨기기
    }

    public void HideItemDescription()
    {
        itemDescriptionText.gameObject.SetActive(false);
    }
}
