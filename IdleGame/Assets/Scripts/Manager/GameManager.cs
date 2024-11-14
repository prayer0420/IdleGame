using System.Collections;
using UnityEngine;

public class GameManager : SingletonDontDestroyOnLoad<GameManager>
{
    [SerializeField] private GameObject playerPrefab;

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

        // ��� ������ �ε� �Ϸ� �� ���� ����
        StartGame();
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
