using UnityEngine;

public class SingletonDontDestroyOnLoad<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    /// 싱글톤 인스턴스에 접근하기 위한 프로퍼티
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬 내에서 해당 타입의 오브젝트를 검색
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    // 없을 경우 새 게임 오브젝트를 생성하여 인스턴스로 설정
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString();

                    // 씬 전환 시 파괴되지 않도록 설정
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    /// 중복 생성을 방지하기 위한 초기화
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
