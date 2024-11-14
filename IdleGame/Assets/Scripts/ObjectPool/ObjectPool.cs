using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : SingletonDontDestroyOnLoad<ObjectPool>
{
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    public GameObject GetObject(GameObject prefab)
    {
        string key = prefab.name.Replace("(Clone)", "");

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
        }

        GameObject obj;
        if (poolDictionary[key].Count > 0)
        {
            obj = poolDictionary[key].Dequeue();
            if (obj == null)
            {
                // null인 경우 다시 생성
                obj = Instantiate(prefab);
                obj.name = key;
            }
        }
        else
        {
            obj = Instantiate(prefab);
            obj.name = key;
        }

        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        string key = obj.name.Replace("(Clone)", "");

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
        }

        // 오브젝트 초기화
        obj.transform.SetParent(null);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;

        poolDictionary[key].Enqueue(obj);
    }
}
