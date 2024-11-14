using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonDontDestroyOnLoad<MapManager>
{
    public List<GameObject> normalMapPrefabs; // 일반 맵 프리팹 리스트
    public GameObject bossMapPrefab; // 보스 맵 프리팹

    private int currentMapIndex = 0;
    private Queue<GameObject> mapQueue = new Queue<GameObject>();
    private GameObject previousMap; // 이전 맵을 저장

    private bool isLoadingMap = false;

    // 게임 시작 시 첫 맵 로드
    public void StartGame()
    {
        currentMapIndex = 0;
        LoadNextMap(); // 첫 번째 맵 로드
    }

    public void LoadNextMap()
    {
        if (isLoadingMap)
        {
            Debug.LogWarning("Already loading a map.");
            return;
        }
        isLoadingMap = true;

        GameObject mapPrefab;

        if (currentMapIndex < 5)
        {
            // 일반 맵 로드 (랜덤하게)
            if (normalMapPrefabs.Count == 0)
            {
                Debug.LogError("No normal map prefabs assigned.");
                isLoadingMap = false;
                return;
            }
            int randomIndex = Random.Range(0, normalMapPrefabs.Count);
            mapPrefab = normalMapPrefabs[randomIndex];
        }
        else
        {
            // 보스 맵 로드
            if (bossMapPrefab == null)
            {
                Debug.LogError("Boss map prefab not assigned.");
                isLoadingMap = false;
                return;
            }
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // 다음에 다시 일반 맵으로 돌아가도록 설정
        }

        GameObject newMap = ObjectPool.Instance.GetObject(mapPrefab);
        if (newMap == null)
        {
            Debug.LogError($"Failed to get map from ObjectPool for prefab: {mapPrefab.name}");
            isLoadingMap = false;
            return;
        }
        newMap.transform.SetParent(transform); // MapManager의 자식으로 설정
        newMap.SetActive(true); // 새 맵 활성화

        if (previousMap != null)
        {
            // 이전 맵의 Exit 포인트와 새로운 맵의 Entrance 포인트를 맞추기
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");
            if (previousExit != null && newEntrance != null)
            {
                Vector3 positionOffset = (previousExit.position - newEntrance.position) + new Vector3(0, 0, 1.5f);
                newMap.transform.position += positionOffset;
                Debug.Log($"Set new map position with offset: {positionOffset}");

                // 플레이어의 위치 조정
                if (PlayerController.Instance == null)
                {
                    Debug.LogError("PlayerController.Instance is null. Cannot set player position.");
                    isLoadingMap = false;
                    return;
                }
                else
                {
                    PlayerController.Instance.transform.position = newEntrance.position;
                    PlayerController.Instance.transform.rotation = newEntrance.rotation;
                    Debug.Log("Player position set to new Entrance.");
                }
            }
            else
            {
                Debug.LogError("Exit or Entrance point missing in the map prefabs.");
            }

            // 이전 맵 비활성화 및 ObjectPool로 반환
            previousMap.SetActive(false); // 이전 맵 비활성화
            ObjectPool.Instance.ReturnObject(previousMap);
            Debug.Log("Old map deactivated and returned to pool.");
        }

        // 새 맵을 큐에 추가하고 이전 맵으로 설정
        mapQueue.Enqueue(newMap);
        previousMap = newMap;
        currentMapIndex++;

        // 플레이어의 다음 목적지 설정
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetNextDestination();
            Debug.Log("Player's next destination set.");
        }

        // 새로운 맵의 몬스터 스폰
        MapController mapController = newMap.GetComponent<MapController>();
        if (mapController != null)
        {
            mapController.SpawnMonsters();
            Debug.Log("Monsters spawned in the new map.");
        }
        else
        {
            Debug.LogError("MapController not found on the map prefab.");
        }

        isLoadingMap = false;
    }

    // 플레이어가 Exit에 도달했을 때 
    public void OnPlayerReachExit()
    {
        Debug.Log("OnPlayerReachExit called.");
        LoadNextMap();
    }
}
