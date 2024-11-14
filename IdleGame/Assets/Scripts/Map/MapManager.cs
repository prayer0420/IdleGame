using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonDontDestroyOnLoad<MapManager>
{
    public List<GameObject> normalMapPrefabs; // �Ϲ� �� ������ ����Ʈ
    public GameObject bossMapPrefab; // ���� �� ������

    private int currentMapIndex = 0;
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
            Debug.LogWarning("�̹� ���� �ε� ���Դϴ�.");
            return;
        }
        isLoadingMap = true;

        GameObject mapPrefab;

        if (currentMapIndex < 5)
        {
            // �Ϲ� �� �ε� (��ȯ)
            if (normalMapPrefabs.Count == 0)
            {
                Debug.LogError("�Ҵ�� �Ϲ� �� �������� �����ϴ�.");
                isLoadingMap = false;
                return;
            }
            int index = currentMapIndex % normalMapPrefabs.Count;
            mapPrefab = normalMapPrefabs[index];
        }
        else
        {
            // ���� �� �ε�
            if (bossMapPrefab == null)
            {
                Debug.LogError("���� �� �������� �Ҵ���� �ʾҽ��ϴ�.");
                isLoadingMap = false;
                return;
            }
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // ������ �ٽ� �Ϲ� ������ ���ư����� ����
        }

        // ObjectPool���� �� ��������
        GameObject newMap = ObjectPool.Instance.GetObject(mapPrefab);
        if (newMap == null)
        {
            Debug.LogError($"ObjectPool���� {mapPrefab.name} ���� �������� ���߽��ϴ�.");
            isLoadingMap = false;
            return;
        }
        newMap.transform.SetParent(transform); // MapManager�� �ڽ����� ����

        // �� ��ġ ���� (Exit�� Entrance ����)
        if (previousMap != null)
        {
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");
            if (previousExit != null && newEntrance != null)
            {
                Vector3 positionOffset = previousExit.position - newEntrance.position;
                newMap.transform.position += positionOffset;

                // �÷��̾� ��ġ ����
                PlayerController.Instance.transform.position = newEntrance.position + positionOffset;
                PlayerController.Instance.transform.rotation = newEntrance.rotation;
            }

            // ���� �� ��Ȱ��ȭ �� ObjectPool�� ��ȯ
            previousMap.SetActive(false);
            ObjectPool.Instance.ReturnObject(previousMap);
        }

        // �� �� Ȱ��ȭ �� ����
        newMap.SetActive(true);
        previousMap = newMap;
        currentMapIndex++;

        // �÷��̾��� ���� ������ ����
        PlayerController.Instance.SetNextDestination();

        // ���ο� ���� ���� ����
        MapController mapController = newMap.GetComponent<MapController>();
        if (mapController != null)
        {
            mapController.SpawnMonsters();
        }

        isLoadingMap = false;
    }

    // �÷��̾ Exit�� �������� �� 
    public void OnPlayerReachExit()
    {
        LoadNextMap();
    }
}
