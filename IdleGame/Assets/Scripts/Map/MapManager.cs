using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonDontDestroyOnLoad<MapManager>
{
    public List<GameObject> normalMapPrefabs; // 일반 맵 프리팹 리스트
    public GameObject bossMapPrefab; // 보스 맵 프리팹

    private int currentMapIndex = 0;
    private Queue<GameObject> mapQueue = new Queue<GameObject>();
    private GameObject previousMap; // 이전 맵을 저장

    // 게임 시작 시 첫 맵 로드
    public void StartGame()
    {
        currentMapIndex = 0;
        LoadNextMap(); // 첫 번째 맵 로드
    }

    public void LoadNextMap()
    {


        GameObject mapPrefab;

        if (currentMapIndex < 5)
        {
            // 일반 맵 로드 (랜덤하게)
            if (normalMapPrefabs.Count == 0)
            {
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
                return;
            }
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // 다음에 다시 일반 맵으로 돌아가도록 설정
        }

        GameObject newMap = ObjectPool.Instance.GetObject(mapPrefab);
        newMap.transform.SetParent(transform); // MapManager의 자식으로 설정

        if (previousMap != null)
        {
            // 이전 맵의 Exit 포인트와 새로운 맵의 Entrance 포인트를 맞추기
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");
            if (previousExit != null && newEntrance != null)
            {
                Vector3 positionOffset = previousExit.position - newEntrance.position;
                newMap.transform.position += positionOffset;

                // 플레이어가 존재하지 않으면 반환
                if (PlayerController.Instance == null)
                {
                    return;
                }

                // 플레이어의 위치 조정
                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.transform.position = newEntrance.position;
                    PlayerController.Instance.transform.rotation = newEntrance.rotation;
                }
            }
        }
        else
        {
            // 첫 번째 맵은 위치와 회전을 기본값으로 설정
            newMap.transform.position = Vector3.zero;
            newMap.transform.rotation = Quaternion.identity;
        }

        mapQueue.Enqueue(newMap);
        previousMap = newMap;
        currentMapIndex++;

        // 맵 관리 (필요 시 이전 맵 반환)
        if (mapQueue.Count > 3)
        {
            GameObject oldMap = mapQueue.Dequeue();
            ObjectPool.Instance.ReturnObject(oldMap);
        }

        // 플레이어의 다음 목적지 설정
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetNextDestination();
        }

        MapController mapController = newMap.GetComponent<MapController>();
        if (mapController != null)
        {
            mapController.SpawnMonsters();
        }
    }

    // 플레이어가 Exit에 도달했을 때 
    public void OnPlayerReachExit()
    {
        LoadNextMap();
    }
}
