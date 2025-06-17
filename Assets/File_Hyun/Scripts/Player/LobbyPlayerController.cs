using UnityEngine;

public class LobbyPlayerController : MonoBehaviour
{
    public enum LobbyplayerState
    {
        one,
        two,
        three,
        four
    }

    public LobbyplayerState CurrentLobbyPlayer;
}