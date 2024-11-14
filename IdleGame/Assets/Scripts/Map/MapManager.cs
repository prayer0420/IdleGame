using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonDontDestroyOnLoad<MapManager>
{
    public List<GameObject> normalMapPrefabs; // �Ϲ� �� ������ ����Ʈ
    public GameObject bossMapPrefab; // ���� �� ������

    private int currentMapIndex = 0;
    private Queue<GameObject> mapQueue = new Queue<GameObject>();
    private GameObject previousMap; // ���� ���� ����

    // ���� ���� �� ù �� �ε�
    public void StartGame()
    {
        currentMapIndex = 0;
        LoadNextMap(); // ù ��° �� �ε�
    }

    public void LoadNextMap()
    {


        GameObject mapPrefab;

        if (currentMapIndex < 5)
        {
            // �Ϲ� �� �ε� (�����ϰ�)
            if (normalMapPrefabs.Count == 0)
            {
                return;
            }
            int randomIndex = Random.Range(0, normalMapPrefabs.Count);
            mapPrefab = normalMapPrefabs[randomIndex];
        }
        else
        {
            // ���� �� �ε�
            if (bossMapPrefab == null)
            {
                return;
            }
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // ������ �ٽ� �Ϲ� ������ ���ư����� ����
        }

        GameObject newMap = ObjectPool.Instance.GetObject(mapPrefab);
        newMap.transform.SetParent(transform); // MapManager�� �ڽ����� ����

        if (previousMap != null)
        {
            // ���� ���� Exit ����Ʈ�� ���ο� ���� Entrance ����Ʈ�� ���߱�
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");
            if (previousExit != null && newEntrance != null)
            {
                Vector3 positionOffset = previousExit.position - newEntrance.position;
                newMap.transform.position += positionOffset;

                // �÷��̾ �������� ������ ��ȯ
                if (PlayerController.Instance == null)
                {
                    return;
                }

                // �÷��̾��� ��ġ ����
                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.transform.position = newEntrance.position;
                    PlayerController.Instance.transform.rotation = newEntrance.rotation;
                }
            }
        }
        else
        {
            // ù ��° ���� ��ġ�� ȸ���� �⺻������ ����
            newMap.transform.position = Vector3.zero;
            newMap.transform.rotation = Quaternion.identity;
        }

        mapQueue.Enqueue(newMap);
        previousMap = newMap;
        currentMapIndex++;

        // �� ���� (�ʿ� �� ���� �� ��ȯ)
        if (mapQueue.Count > 3)
        {
            GameObject oldMap = mapQueue.Dequeue();
            ObjectPool.Instance.ReturnObject(oldMap);
        }

        // �÷��̾��� ���� ������ ����
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

    // �÷��̾ Exit�� �������� �� 
    public void OnPlayerReachExit()
    {
        LoadNextMap();
    }
}
