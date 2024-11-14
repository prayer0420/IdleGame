using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.OnPlayerReachExit();
        }
    }
}