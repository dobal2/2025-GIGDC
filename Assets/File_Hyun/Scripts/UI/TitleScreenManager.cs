using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    void Start()
    {
        SaveKey.Instance.LoadKeyBindings();
        InputManager.Instance.currentContext = InputManager.InputContext.UI;
    }

    public void Continue()
    {
        Debug.Log("이어하기");
        Stage.LoadData();
        SceneManager.LoadScene("Lobby_Over");
    }

    public void NewGame()
    {
        Debug.Log("처음부터");
        Stage.Data = StageDataType.Start;
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