using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterDatabase : SingletonDontDestroyOnLoad<CharacterDatabase>
{
    public CharacterData playerData;

    public void Start()
    {
        StartCoroutine(LoadCharacterDataFromGoogleSheet());
    }

    private IEnumerator LoadCharacterDataFromGoogleSheet()
    {
        string url = "https://script.google.com/macros/s/AKfycbzLRToIEI44n23liWAisvYL7tFYBL3lq70RmLal74ye8WFsB2yvRchb7JqXthT_HgPd/exec?sheet=Characters";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading character data: " + www.error);
        }
        else
        {
            string jsonData = www.downloadHandler.text;
            Debug.Log("Received JSON for Characters: " + jsonData); // JSON ������ ���

            CharacterDataWrapper wrapper = JsonUtility.FromJson<CharacterDataWrapper>(jsonData);
            playerData = wrapper.Characters[0];
        }
    }
}

[Serializable]
public class CharacterDataWrapper
{
    public List<CharacterData> Characters;
}

