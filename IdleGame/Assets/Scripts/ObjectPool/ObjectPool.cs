using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : SingletonDontDestroyOnLoad<ObjectPool>
{
    // �������� Ű�� ����Ͽ� ť�� ����
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    }

    // Ư�� �������� �̸� �ε��ϴ� �޼���
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

            // PrefabIdentifier �߰� �Ǵ� ����
            PrefabIdentifier identifier = obj.GetComponent<PrefabIdentifier>();
            if (identifier == null)
            {
                identifier = obj.AddComponent<PrefabIdentifier>();
            }
            identifier.prefab = prefab;

            poolDictionary[prefab].Enqueue(obj);
        }
    }

    // ObjectPool���� ������Ʈ�� �������� �޼���
    public GameObject GetObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogError($"Ǯ�� {prefab.name} �������� �����ϴ�. �̸� �ε�Ǿ����� Ȯ���ϼ���.");
            return null;
        }

        GameObject obj;
        if (poolDictionary[prefab].Count > 0)
        {
            obj = poolDictionary[prefab].Dequeue();
        }
        else
        {
            Debug.LogWarning($"{prefab.name}�� ������Ʈ�� ��� ��� ���Դϴ�.");
            return null;
        }

        obj.SetActive(true);
        return obj;
    }

    // ObjectPool�� ������Ʈ�� ��ȯ�ϴ� �޼���
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);

        // PrefabIdentifier�� ���� ���� �������� �ĺ�
        PrefabIdentifier identifier = obj.GetComponent<PrefabIdentifier>();
        if (identifier == null)
        {
            Debug.LogError($"Returned object {obj.name}�� PrefabIdentifier�� �����ϴ�. ������Ʈ�� �ı��մϴ�.");
            Destroy(obj);
            return;
        }

        GameObject prefab = identifier.prefab;

        if (prefab == null)
        {
            Debug.LogError($"PrefabIdentifier�� {obj.name}�� �������� �������� �ʽ��ϴ�. ������Ʈ�� �ı��մϴ�.");
            Destroy(obj);
            return;
        }

        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogError($"Ǯ�� {prefab.name} �������� �����ϴ�. ��ȯ�� �� �����ϴ�.");
            Destroy(obj);
            return;
        }

        // ������Ʈ �ʱ�ȭ
        obj.transform.SetParent(null);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;

        poolDictionary[prefab].Enqueue(obj);
    }
}
