using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ItemDatabase : SingletonDontDestroyOnLoad<ItemDatabase>
{
    public List<ItemData> ItemDatas = new List<ItemData>();

    public void Start()
    {
        StartCoroutine(LoadItemsFromGoogleSheet());
    }

    private IEnumerator LoadItemsFromGoogleSheet()
    {
        //구글 스프레드 URL
        string url = "https://script.google.com/macros/s/AKfycbzLRToIEI44n23liWAisvYL7tFYBL3lq70RmLal74ye8WFsB2yvRchb7JqXthT_HgPd/exec?sheet=Items"; 

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading items: " + www.error);
        }
        else
        {
            string jsonData = www.downloadHandler.text;
            // JSON 데이터 역직렬화
            ItemDataListWrapper wrapper = JsonUtility.FromJson<ItemDataListWrapper>(jsonData);
            ItemDatas = wrapper.Items;
        }
    }
}

[Serializable]
public class ItemDataListWrapper
{
    public List<ItemData> Items;
}
