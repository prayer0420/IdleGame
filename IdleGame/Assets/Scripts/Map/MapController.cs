using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> monsterPrefabs; // 스폰할 몬스터 프리팹 리스트
    public int monsterCount; // 스폰할 몬스터 수
    public float minSpawnDistanceFromPlayer = 5f; // 플레이어로부터 최소 스폰 거리

    [SerializeField] public List<GameObject> spawnedMonsters = new List<GameObject>();
    private Collider mapCollider; // 맵의 콜라이더

    // Awake에서 mapCollider를 할당
    public void Awake()
    {
        mapCollider = GetComponent<Collider>();
        if (mapCollider == null)
        {
            Debug.LogError("MapController에 Collider가 없습니다.");
        }
    }

    public void Start()
    {
        //// 출구 비활성화
        //GameObject exitPoint = transform.Find("Exit")?.gameObject;
        //if (exitPoint != null)
        //{
        //    exitPoint.SetActive(false);
        //}
        //SpawnMonsters();
    }

    public void SpawnMonsters()
    {
        if (monsterPrefabs == null || monsterPrefabs.Count == 0)
        {
            Debug.Log("적 생성 이상");
            return;
        }

        for (int i = 0; i < monsterCount; i++)
        {
            int randomIndex = Random.Range(0, monsterPrefabs.Count);
            GameObject monsterPrefab = monsterPrefabs[randomIndex];
            GameObject monster = ObjectPool.Instance.GetObject(monsterPrefab);

            if (monster == null)
            {
                continue;
            }

            Vector3 randomPosition = GetRandomPositionWithinMap();
            monster.transform.position = randomPosition;
            monster.transform.rotation = Quaternion.identity;

            // 몬스터에게 현재 맵 정보를 전달
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
                //ActivateExit();
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
    }

    //public void OnDisable()
    //{
    //    foreach (GameObject monster in spawnedMonsters)
    //    {
    //        ObjectPool.Instance.ReturnObject(monster);
    //    }
    //    spawnedMonsters.Clear();
    //}
}
