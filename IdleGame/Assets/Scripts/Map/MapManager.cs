using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonDontDestroyOnLoad<MapManager>
{
    public List<GameObject> normalMapPrefabs; // �Ϲ� �� ������ ����Ʈ
    public GameObject bossMapPrefab; // ���� �� ������

    private int currentMapIndex = 0;
    private Queue<GameObject> mapQueue = new Queue<GameObject>();
    private GameObject previousMap; // ���� ���� ����

    private bool isLoadingMap = false;

    // ���� ���� �� ù �� �ε�
    public void StartGame()
    {
        currentMapIndex = 0;
        LoadNextMap(); // ù ��° �� �ε�
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
            // �Ϲ� �� �ε� (�����ϰ�)
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
            // ���� �� �ε�
            if (bossMapPrefab == null)
            {
                Debug.LogError("Boss map prefab not assigned.");
                isLoadingMap = false;
                return;
            }
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // ������ �ٽ� �Ϲ� ������ ���ư����� ����
        }

        GameObject newMap = ObjectPool.Instance.GetObject(mapPrefab);
        if (newMap == null)
        {
            Debug.LogError($"Failed to get map from ObjectPool for prefab: {mapPrefab.name}");
            isLoadingMap = false;
            return;
        }
        newMap.transform.SetParent(transform); // MapManager�� �ڽ����� ����
        newMap.SetActive(true); // �� �� Ȱ��ȭ

        if (previousMap != null)
        {
            // ���� ���� Exit ����Ʈ�� ���ο� ���� Entrance ����Ʈ�� ���߱�
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");
            if (previousExit != null && newEntrance != null)
            {
                Vector3 positionOffset = (previousExit.position - newEntrance.position) + new Vector3(0, 0, 1.5f);
                newMap.transform.position += positionOffset;
                Debug.Log($"Set new map position with offset: {positionOffset}");

                // �÷��̾��� ��ġ ����
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

            // ���� �� ��Ȱ��ȭ �� ObjectPool�� ��ȯ
            previousMap.SetActive(false); // ���� �� ��Ȱ��ȭ
            ObjectPool.Instance.ReturnObject(previousMap);
            Debug.Log("Old map deactivated and returned to pool.");
        }

        // �� ���� ť�� �߰��ϰ� ���� ������ ����
        mapQueue.Enqueue(newMap);
        previousMap = newMap;
        currentMapIndex++;

        // �÷��̾��� ���� ������ ����
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetNextDestination();
            Debug.Log("Player's next destination set.");
        }

        // ���ο� ���� ���� ����
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

    // �÷��̾ Exit�� �������� �� 
    public void OnPlayerReachExit()
    {
        Debug.Log("OnPlayerReachExit called.");
        LoadNextMap();
    }
}
