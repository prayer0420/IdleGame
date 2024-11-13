using UnityEngine;

public class GameManager : SingletonDontDestroyOnLoad<GameManager>
{
    public void Start()
    {
        MapManager.Instance.StartGame();
    }
}
