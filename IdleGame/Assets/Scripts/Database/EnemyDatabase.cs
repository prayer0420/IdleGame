using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyDatabase : SingletonDontDestroyOnLoad<EnemyDatabase>
{
    public List<EnemyData> EnemyDatas= new List<EnemyData>();

    public void Start()
    {
        StartCoroutine(LoadCharacterDataFromGoogleSheet());
    }

    private IEnumerator LoadCharacterDataFromGoogleSheet()
    {
        string url = "https://script.google.com/macros/s/AKfycbzLRToIEI44n23liWAisvYL7tFYBL3lq70RmLal74ye8WFsB2yvRchb7JqXthT_HgPd/exec?sheet=Enemies";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading Enemys data: " + www.error);
        }
        else
        {
            string jsonData = www.downloadHandler.text;
            EnemyDatatWrapper wrapper = JsonUtility.FromJson<EnemyDatatWrapper>(jsonData);
            EnemyDatas = wrapper.Enemies;
        }
    }
}

[Serializable]
public class EnemyDatatWrapper
{
    public List<EnemyData> Enemies;
}
