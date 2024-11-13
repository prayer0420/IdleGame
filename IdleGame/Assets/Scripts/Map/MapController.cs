using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> monsterPrefabs; // 스폰할 몬스터 프리팹 리스트
    public int monsterCount = 5; // 스폰할 몬스터 수
    public float minSpawnDistanceFromPlayer = 5f; // 플레이어로부터 최소 스폰 거리

    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private Collider mapCollider; // 맵의 콜라이더

    public void Start()
    {
        mapCollider = GetComponent<Collider>();
        if (mapCollider == null)
        {
            Debug.LogError("MapController에 Collider가 없습니다.");
            return;
        }

        SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        for (int i = 0; i < monsterCount; i++)
        {
            int randomIndex = Random.Range(0, monsterPrefabs.Count);
            GameObject monster = ObjectPool.Instance.GetObject(monsterPrefabs[randomIndex]);

            Vector3 randomPosition = GetRandomPositionWithinMap();
            monster.transform.position = randomPosition;
            monster.transform.rotation = Quaternion.identity;
            monster.SetActive(true);
            spawnedMonsters.Add(monster);

            // 몬스터에게 현재 맵 정보를 전달
            EnemyController enemyController = monster.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.currentMap = this;
            }
        }
    }

    private Vector3 GetRandomPositionWithinMap()
    {
        Bounds bounds = mapCollider.bounds;
        Vector3 randomPosition;

        int maxAttempts = 10;
        int attempts = 0;
        bool validPosition = false;

        do
        {
            float posX = Random.Range(bounds.min.x, bounds.max.x);
            float posZ = Random.Range(bounds.min.z, bounds.max.z);
            float posY = -1.65f;

            randomPosition = new Vector3(posX, posY, posZ);

            if (PlayerController.Instance != null)
            {
                float distanceToPlayer = Vector3.Distance(randomPosition, PlayerController.Instance.transform.position);
                if (distanceToPlayer < minSpawnDistanceFromPlayer)
                {
                    validPosition = false;
                }
                else
                {
                    validPosition = true;
                }
            }
            else
            {
                validPosition = true;
            }

            attempts++;
        }
        while (!validPosition && attempts < maxAttempts);

        return randomPosition;
    }

    public void OnMonsterDeath(GameObject monster)
    {
        spawnedMonsters.Remove(monster);
        ObjectPool.Instance.ReturnObject(monster);

        if (spawnedMonsters.Count == 0)
        {
            // 모든 몬스터가 제거되었을 때 MapManager에 알림
            MapManager.Instance.OnAllMonstersDefeated();
        }
    }

    public void OnDisable()
    {
        // 맵이 비활성화될 때 남은 몬스터를 모두 반환
        foreach (GameObject monster in spawnedMonsters)
        {
            ObjectPool.Instance.ReturnObject(monster);
        }
        spawnedMonsters.Clear();
    }
}
