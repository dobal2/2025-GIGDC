using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    void Start()
    {
        SaveKey.Instance.LoadKeyBindings();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Hyun_TestScene");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}