using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    private void Start()
    {
        PlayerController.Instance.transform.position = transform.position;
    }
}
