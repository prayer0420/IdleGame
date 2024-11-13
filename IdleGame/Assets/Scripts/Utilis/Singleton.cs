using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
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
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
