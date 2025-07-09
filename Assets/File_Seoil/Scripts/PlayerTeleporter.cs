using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    private void Start()
    {
        if(PlayerController.Instance == null)
            LobbyPlayerController.Instance.transform.position = transform.position;
        else 
            PlayerController.Instance.transform.position = transform.position;
    }
}
