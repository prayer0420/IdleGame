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
            int randomIndex = Random.Range(0, normalMapPrefabs.Count);
            mapPrefab = normalMapPrefabs[randomIndex];
        }
        else
        {
            // 보스 맵 로드
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // 다음에 다시 일반 맵으로 돌아가도록 설정
        }

        // ObjectPool을 통해 맵 인스턴스 가져오기
        GameObject newMap = ObjectPool.Instance.GetObject(mapPrefab);
        newMap.transform.SetParent(transform); // MapManager의 자식으로 설정

        if (previousMap != null)
        {
            // 이전 맵의 Exit 포인트와 새로운 맵의 Entrance 포인트를 맞추기
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");

            if (previousExit != null && newEntrance != null)
            {
                // 이전 Exit의 월드 위치
                Vector3 previousExitWorldPos = previousExit.position;

                // 새로운 Entrance의 월드 위치를 이전 Exit 위치로 맞추기 위해 오프셋 계산
                Vector3 positionOffset = previousExitWorldPos - newEntrance.position;

                newMap.transform.position += positionOffset;

                Debug.Log($"Previous Exit Position: {previousExitWorldPos}, New Entrance Position: {newEntrance.position}, Position Offset: {positionOffset}, New Map Position: {newMap.transform.position}");
            }
            else
            {
                Debug.LogError("Exit 또는 Entrance 포인트가 맵 프리팹에 존재하지 않습니다.");
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

        // 플레이어 위치 설정 (새로운 맵이 로드된 후)
        PlayerController.Instance.SetStartPosition();
    }

    // 플레이어가 Exit에 도달했을 때 호출되는 메소드
    public void OnPlayerReachExit()
    {
        LoadNextMap();
    }

    // 현재 맵의 몬스터가 모두 제거되었을 때 호출되는 메소드
    public void OnAllMonstersDefeated()
    {
        LoadNextMap();
    }
}
