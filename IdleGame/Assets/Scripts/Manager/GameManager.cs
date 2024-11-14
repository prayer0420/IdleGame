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
        // 게임 시작을 위한 초기화 코루틴 시작
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        // 데이터 로딩 시작
        yield return StartCoroutine(CharacterDatabase.Instance.LoadCharacterDataFromGoogleSheet());
        yield return StartCoroutine(EnemyDatabase.Instance.LoadEnemiesDataFromGoogleSheet());
        yield return StartCoroutine(ItemDatabase.Instance.LoadItemsFromGoogleSheet());

        // 모든 데이터 로딩 완료 후 게임 시작
        StartGame();
    }

    private void StartGame()
    {

        // 플레이어 스폰
        SpawnPlayer();
        // 맵 생성
        MapManager.Instance.StartGame();


    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            return;
        }

        // 플레이어의 시작 위치는 첫 맵의 Entrance 위치로 설정
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
