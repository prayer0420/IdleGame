using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : SingletonDontDestroyOnLoad<ObjectPool>
{
    // 프리팹을 키로 사용하여 큐를 관리
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    }

    // 특정 프리팹을 미리 로드하는 메서드
    public void PreloadObjects(GameObject prefab, int count)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab] = new Queue<GameObject>();
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.name = prefab.name;

            // PrefabIdentifier 추가 또는 설정
            PrefabIdentifier identifier = obj.GetComponent<PrefabIdentifier>();
            if (identifier == null)
            {
                identifier = obj.AddComponent<PrefabIdentifier>();
            }
            identifier.prefab = prefab;

            poolDictionary[prefab].Enqueue(obj);
        }
    }

    // ObjectPool에서 오브젝트를 가져오는 메서드
    public GameObject GetObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogError($"풀에 {prefab.name} 프리팹이 없습니다. 미리 로드되었는지 확인하세요.");
            return null;
        }

        GameObject obj;
        if (poolDictionary[prefab].Count > 0)
        {
            obj = poolDictionary[prefab].Dequeue();
        }
        else
        {
            Debug.LogWarning($"{prefab.name}의 오브젝트가 모두 사용 중입니다.");
            return null;
        }

        obj.SetActive(true);
        return obj;
    }

    // ObjectPool에 오브젝트를 반환하는 메서드
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);

        // PrefabIdentifier를 통해 원본 프리팹을 식별
        PrefabIdentifier identifier = obj.GetComponent<PrefabIdentifier>();
        if (identifier == null)
        {
            Debug.LogError($"Returned object {obj.name}에 PrefabIdentifier가 없습니다. 오브젝트를 파괴합니다.");
            Destroy(obj);
            return;
        }

        GameObject prefab = identifier.prefab;

        if (prefab == null)
        {
            Debug.LogError($"PrefabIdentifier가 {obj.name}에 프리팹을 참조하지 않습니다. 오브젝트를 파괴합니다.");
            Destroy(obj);
            return;
        }

        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogError($"풀에 {prefab.name} 프리팹이 없습니다. 반환할 수 없습니다.");
            Destroy(obj);
            return;
        }

        // 오브젝트 초기화
        obj.transform.SetParent(null);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;

        poolDictionary[prefab].Enqueue(obj);
    }
}
