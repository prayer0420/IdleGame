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

    public void Start()
    {
        // �ⱸ ��Ȱ��ȭ
        GameObject exitPoint = transform.Find("Exit")?.gameObject;
        if (exitPoint != null)
        {
            exitPoint.SetActive(false);
        }
        else
        {
            Debug.LogError("Exit ����Ʈ�� �� �����տ� �������� �ʽ��ϴ�.");
        }

        SpawnMonsters();
    }

    public void SpawnMonsters()
    {
        if (monsterPrefabs == null || monsterPrefabs.Count == 0)
        {
            Debug.LogWarning("���� �������� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        for (int i = 0; i < monsterCount; i++)
        {
            int randomIndex = Random.Range(0, monsterPrefabs.Count);
            GameObject monsterPrefab = monsterPrefabs[randomIndex];
            GameObject monster = ObjectPool.Instance.GetObject(monsterPrefab);

            if (monster == null)
            {
                Debug.LogWarning("���� �ν��Ͻ��� ������ �� �����ϴ�.");
                continue;
            }

            Vector3 randomPosition = GetRandomPositionWithinMap();
            monster.transform.position = randomPosition;
            monster.transform.rotation = Quaternion.identity;

            // ���Ϳ��� ���� �� ������ ����
            EnemyController enemyController = monster.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.currentMap = this;
                spawnedMonsters.Add(monster);
            }
            else
            {
                ObjectPool.Instance.ReturnObject(monster);
                Debug.LogError("���� �����տ� EnemyController�� �����ϴ�.");
            }
        }
    }

    private Vector3 GetRandomPositionWithinMap()
    {
        if (mapCollider == null)
        {
            Debug.LogError("MapController�� Collider�� �������� �ʾҽ��ϴ�.");
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
            Debug.LogWarning("��ȿ�� ���� ��ġ�� ã�� ���߽��ϴ�. �⺻ ��ġ ���.");
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
            Debug.Log("Exit point activated.");
        }
        else
        {
            Debug.LogError("Exit ����Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void OnDisable()
    {
        foreach (GameObject monster in spawnedMonsters)
        {
            ObjectPool.Instance.ReturnObject(monster);
        }
        spawnedMonsters.Clear();
    }
}
