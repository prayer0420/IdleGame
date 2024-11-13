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
            int randomIndex = Random.Range(0, normalMapPrefabs.Count);
            mapPrefab = normalMapPrefabs[randomIndex];
        }
        else
        {
            // ���� �� �ε�
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // ������ �ٽ� �Ϲ� ������ ���ư����� ����
        }

        // ObjectPool�� ���� �� �ν��Ͻ� ��������
        GameObject newMap = ObjectPool.Instance.GetObject(mapPrefab);
        newMap.transform.SetParent(transform); // MapManager�� �ڽ����� ����

        if (previousMap != null)
        {
            // ���� ���� Exit ����Ʈ�� ���ο� ���� Entrance ����Ʈ�� ���߱�
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");

            if (previousExit != null && newEntrance != null)
            {
                // ���� Exit�� ���� ��ġ
                Vector3 previousExitWorldPos = previousExit.position;

                // ���ο� Entrance�� ���� ��ġ�� ���� Exit ��ġ�� ���߱� ���� ������ ���
                Vector3 positionOffset = previousExitWorldPos - newEntrance.position;

                newMap.transform.position += positionOffset;

                Debug.Log($"Previous Exit Position: {previousExitWorldPos}, New Entrance Position: {newEntrance.position}, Position Offset: {positionOffset}, New Map Position: {newMap.transform.position}");
            }
            else
            {
                Debug.LogError("Exit �Ǵ� Entrance ����Ʈ�� �� �����տ� �������� �ʽ��ϴ�.");
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

        // �÷��̾� ��ġ ���� (���ο� ���� �ε�� ��)
        PlayerController.Instance.SetStartPosition();
    }

    // �÷��̾ Exit�� �������� �� ȣ��Ǵ� �޼ҵ�
    public void OnPlayerReachExit()
    {
        LoadNextMap();
    }

    // ���� ���� ���Ͱ� ��� ���ŵǾ��� �� ȣ��Ǵ� �޼ҵ�
    public void OnAllMonstersDefeated()
    {
        LoadNextMap();
    }
}
