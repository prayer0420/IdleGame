using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonDontDestroyOnLoad<MapManager>
{
    public List<GameObject> normalMapPrefabs; // 일반 맵 프리팹 리스트
    public GameObject bossMapPrefab; // 보스 맵 프리팹

    private int currentMapIndex = 0;
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
            Debug.LogWarning("이미 맵을 로드 중입니다.");
            return;
        }
        isLoadingMap = true;

        GameObject mapPrefab;

        if (currentMapIndex < 5)
        {
            // 일반 맵 로드 (순환)
            if (normalMapPrefabs.Count == 0)
            {
                Debug.LogError("할당된 일반 맵 프리팹이 없습니다.");
                isLoadingMap = false;
                return;
            }
            int index = currentMapIndex % normalMapPrefabs.Count;
            mapPrefab = normalMapPrefabs[index];
        }
        else
        {
            // 보스 맵 로드
            if (bossMapPrefab == null)
            {
                Debug.LogError("보스 맵 프리팹이 할당되지 않았습니다.");
                isLoadingMap = false;
                return;
            }
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // 다음에 다시 일반 맵으로 돌아가도록 설정
        }

        // ObjectPool에서 맵 가져오기
        GameObject newMap = ObjectPool.Instance.GetObject(mapPrefab);
        if (newMap == null)
        {
            Debug.LogError($"ObjectPool에서 {mapPrefab.name} 맵을 가져오지 못했습니다.");
            isLoadingMap = false;
            return;
        }
        newMap.transform.SetParent(transform); // MapManager의 자식으로 설정

        // 맵 위치 조정 (Exit와 Entrance 연결)
        if (previousMap != null)
        {
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");
            if (previousExit != null && newEntrance != null)
            {
                Vector3 positionOffset = previousExit.position - newEntrance.position;
                newMap.transform.position += positionOffset;

                // 플레이어 위치 조정
                PlayerController.Instance.transform.position = newEntrance.position + positionOffset;
                PlayerController.Instance.transform.rotation = newEntrance.rotation;
            }

            // 이전 맵 비활성화 및 ObjectPool로 반환
            previousMap.SetActive(false);
            ObjectPool.Instance.ReturnObject(previousMap);
        }

        // 새 맵 활성화 및 설정
        newMap.SetActive(true);
        previousMap = newMap;
        currentMapIndex++;

        // 플레이어의 다음 목적지 설정
        PlayerController.Instance.SetNextDestination();

        // 새로운 맵의 몬스터 스폰
        MapController mapController = newMap.GetComponent<MapController>();
        if (mapController != null)
        {
            mapController.SpawnMonsters();
        }

        isLoadingMap = false;
    }

    // 플레이어가 Exit에 도달했을 때 
    public void OnPlayerReachExit()
    {
        LoadNextMap();
    }
}
