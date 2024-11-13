using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public List<GameObject> normalMapPrefabs; // �Ϲ� �� ������ ����Ʈ
    public GameObject bossMapPrefab; // ���� �� ������

    private int currentMapIndex = 0;
    private Queue<GameObject> mapQueue = new Queue<GameObject>();
    private GameObject previousMap; // ���� ���� ����

    private void Awake()
    {
        instance = this;
    }

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
            // �Ϲ� �� �ε� (������� �Ǵ� �����ϰ�)
            int randomIndex = Random.Range(0, normalMapPrefabs.Count);
            mapPrefab = normalMapPrefabs[randomIndex];
        }
        else
        {
            // ���� �� �ε�
            mapPrefab = bossMapPrefab;
            currentMapIndex = -1; // ������ �ٽ� �Ϲ� ������ ���ư����� ����
        }

        GameObject newMap = Instantiate(mapPrefab);

        if (previousMap != null)
        {
            // ���� ���� ExitPoint�� ���ο� ���� EntrancePoint�� ���߱�
            Transform previousExit = previousMap.transform.Find("Exit");
            Transform newEntrance = newMap.transform.Find("Entrance");

            // ��ġ �� ȸ�� ���
            Vector3 positionOffset = previousExit.position - newEntrance.position;
            newMap.transform.position += positionOffset;

            // �ʿ��� ��� ȸ�� ����
            Quaternion rotationOffset = previousExit.rotation * Quaternion.Inverse(newEntrance.rotation);
            newMap.transform.rotation = rotationOffset * newMap.transform.rotation;
            newMap.transform.position = previousExit.position;
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

        // �� ���� (�ʿ� �� ���� �� ����)
        if (mapQueue.Count > 3)
        {
            GameObject oldMap = mapQueue.Dequeue();
            Destroy(oldMap);
        }
    }
}
