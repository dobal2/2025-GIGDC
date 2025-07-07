using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    void Start()
    {
        SaveKey.Instance.LoadKeyBindings();
        InputManager.Instance.currentContext = InputManager.InputContext.UI;
    }

    public void StartGame()
    {
        Debug.Log("게임 시작 버튼 클릭됨");
        SceneManager.LoadScene("Lobby_Over");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}