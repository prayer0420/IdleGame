using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public List<GameObject> normalMapPrefabs; // 일반 맵 프리팹 리스트
    public GameObject bossMapPrefab; // 보스 맵 프리팹

    private int currentMapIndex = 0;
    private Queue<GameObject> mapQueue = new Queue<GameObject>();
    private GameObject previousMap; // 이전 맵을 저장

    private void Awake()
    {
        instance = this;
    }

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
            // 일반 맵 로드 (순서대로 또는 랜덤하게)
            int randomIndex = Random.Range(0, normalMapPrefabs.Count);
            mapPrefab = normalMapPrefabs[randomIndex];
        }
        else
        {
            // 보스 맵 로드
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // 다음에 다시 일반 맵으로 돌아가도록 설정
        }

        GameObject newMap = Instantiate(mapPrefab);

        if (previousMap != null)
        {
            // 이전 맵의 ExitPoint와 새로운 맵의 EntrancePoint를 맞추기
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");

            // 위치 및 회전 계산
            Vector3 positionOffset = previousExit.position - newEntrance.position;
            newMap.transform.position += positionOffset;

            // 필요한 경우 회전 적용
            Quaternion rotationOffset = previousExit.rotation * Quaternion.Inverse(newEntrance.rotation);
            newMap.transform.rotation = rotationOffset * newMap.transform.rotation;
            newMap.transform.position = previousExit.position;
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

        // 맵 관리 (필요 시 이전 맵 제거)
        if (mapQueue.Count > 3)
        {
            GameObject oldMap = mapQueue.Dequeue();
            Destroy(oldMap);
        }
    }
}
