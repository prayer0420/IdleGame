using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> monsterPrefabs; // ������ ���� ������ ����Ʈ
    public int monsterCount; // ������ ���� ��
    public float minSpawnDistanceFromPlayer = 5f; // �÷��̾�κ��� �ּ� ���� �Ÿ�

    [SerializeField] public List<GameObject> spawnedMonsters = new List<GameObject>();
    private Collider mapCollider; // ���� �ݶ��̴�

    // Awake���� mapCollider�� �Ҵ�
    public void Awake()
    {
        mapCollider = GetComponent<Collider>();
        if (mapCollider == null)
        {
            Debug.LogError("MapController�� Collider�� �����ϴ�.");
        }
    }

    public void SpawnMonsters()
    {
        if (monsterPrefabs == null || monsterPrefabs.Count == 0)
        {
            Debug.Log("�� ���� �̻�");
            return;
        }

        for (int i = 0; i < monsterCount; i++)
        {
            int index = i % monsterPrefabs.Count;
            GameObject monsterPrefab = monsterPrefabs[index];
            GameObject monster = ObjectPool.Instance.GetObject(monsterPrefab);

            if (monster == null)
            {
                Debug.LogWarning($"{monsterPrefab.name} ���͸� ������ �� �����ϴ�.");
                continue;
            }

            Vector3 randomPosition = GetRandomPositionWithinMap();
            monster.transform.position = randomPosition;
            monster.transform.rotation = Quaternion.identity;

            // ���� �ʱ�ȭ
            EnemyController enemyController = monster.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.currentMap = this;
                spawnedMonsters.Add(monster);
            }
            else
            {
                ObjectPool.Instance.ReturnObject(monster);
            }
        }
    }

    private Vector3 GetRandomPositionWithinMap()
    {
        if (mapCollider == null)
        {
            return Vector3.zero;
        }

        Bounds bounds = mapCollider.bounds;
        Vector3 randomPosition;

        int maxAttempts = 10;
        int attempts = 0;
        bool validPosition = false;

        do
        {
            float posX = Random.Range(bounds.min.x, bounds.max.x);
            float posZ = Random.Range(bounds.min.z, bounds.max.z);
            float posY = 0;

            randomPosition = new Vector3(posX, posY, posZ);

            if (PlayerController.Instance != null)
            {
                float distanceToPlayer = Vector3.Distance(randomPosition, PlayerController.Instance.transform.position);
                validPosition = distanceToPlayer >= minSpawnDistanceFromPlayer;
            }
            else
            {
                validPosition = true;
            }

            attempts++;
        }
        while (!validPosition && attempts < maxAttempts);

        if (!validPosition)
        {
            randomPosition = bounds.center;
        }

        return randomPosition;
    }

    public void OnMonsterDeath(GameObject monster)
    {
        if (spawnedMonsters.Contains(monster))
        {
            spawnedMonsters.Remove(monster);
            ObjectPool.Instance.ReturnObject(monster);

            if (spawnedMonsters.Count == 0)
            {
                // ��� ���Ͱ� ���ŵǾ��� �� �ⱸ Ȱ��ȭ
                ActivateExit();
            }
        }
    }

    private void ActivateExit()
    {
        GameObject exitPoint = transform.Find("Exit")?.gameObject;
        if (exitPoint != null)
        {
            exitPoint.SetActive(true);
        }
        else
        {
            Debug.LogError("Exit ����Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void OnDisable()
    {
        // ���� ��Ȱ��ȭ�� �� ���� ���͸� ��� ��ȯ
        foreach (GameObject monster in spawnedMonsters)
        {
            ObjectPool.Instance.ReturnObject(monster);
        }
        spawnedMonsters.Clear();
    }
}
