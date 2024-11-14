using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonDontDestroyOnLoad<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<GameObject> mapPrefabsToPreload; // �̸� �ε��� �� ������ ����Ʈ
    [SerializeField] private List<GameObject> monsterPrefabsToPreload; // �̸� �ε��� ���� ������ ����Ʈ
    private int preloadCount = 3; // �̸� �ε��� ������Ʈ ��

    protected override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        // ���� ������ ���� �ʱ�ȭ �ڷ�ƾ ����
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        // ������ �ε� ����
        yield return StartCoroutine(CharacterDatabase.Instance.LoadCharacterDataFromGoogleSheet());
        yield return StartCoroutine(EnemyDatabase.Instance.LoadEnemiesDataFromGoogleSheet());
        yield return StartCoroutine(ItemDatabase.Instance.LoadItemsFromGoogleSheet());

        // ������ �̸� �ε�
        PreloadPrefabs();

        // ��� ������ �ε� �Ϸ� �� ���� ����
        StartGame();
    }

    private void PreloadPrefabs()
    {
        // �� ������ �̸� �ε�
        foreach (GameObject mapPrefab in mapPrefabsToPreload)
        {
            ObjectPool.Instance.PreloadObjects(mapPrefab, preloadCount);
        }

        // ���� ������ �̸� �ε�
        foreach (GameObject monsterPrefab in monsterPrefabsToPreload)
        {
            ObjectPool.Instance.PreloadObjects(monsterPrefab, preloadCount);
        }
    }

    private void StartGame()
    {
        // �÷��̾� ����
        SpawnPlayer();
        // �� ����
        MapManager.Instance.StartGame();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            return;
        }

        // �÷��̾��� ���� ��ġ�� ù ���� Entrance ��ġ�� ����
        GameObject entrance = GameObject.FindGameObjectWithTag("Entrance");
        Vector3 spawnPosition = entrance != null ? entrance.transform.position : Vector3.zero;
        Quaternion spawnRotation = entrance != null ? entrance.transform.rotation : Quaternion.identity;

        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, spawnRotation);
        if (playerInstance != null)
        {
            playerInstance.name = playerPrefab.name;
        }
    }
}
