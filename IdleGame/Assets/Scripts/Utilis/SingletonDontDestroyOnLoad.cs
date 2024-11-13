using UnityEngine;

public class SingletonDontDestroyOnLoad<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    /// �̱��� �ν��Ͻ��� �����ϱ� ���� ������Ƽ
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // �� ������ �ش� Ÿ���� ������Ʈ�� �˻�
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    // ���� ��� �� ���� ������Ʈ�� �����Ͽ� �ν��Ͻ��� ����
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString();

                    // �� ��ȯ �� �ı����� �ʵ��� ����
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    /// �ߺ� ������ �����ϱ� ���� �ʱ�ȭ
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
