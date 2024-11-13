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

    // 오브젝트를 가져오는 일반 메소드
    public GameObject GetObject(GameObject prefab)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
        }

        if (poolDictionary[key].Count > 0)
        {
            GameObject obj = poolDictionary[key].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab);
            obj.name = key;
            return obj;
        }
    }

    // 오브젝트를 반환하는 일반 메소드
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        if (!poolDictionary.ContainsKey(obj.name))
        {
            poolDictionary[obj.name] = new Queue<GameObject>();
        }
        poolDictionary[obj.name].Enqueue(obj);
    }

}
