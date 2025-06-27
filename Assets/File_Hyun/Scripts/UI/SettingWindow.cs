using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static InputManager;

public class SettingWindow : MonoBehaviour
{
    public static SettingWindow Instance { get; private set; }

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject FirstButton;

    InputContext originalContext;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OpenSetting()
    {
        settingPanel.SetActive(true);
        Time.timeScale = 0;
        originalContext = InputManager.Instance.currentContext;
        InputManager.Instance.currentContext = InputContext.UI;
        EventSystem.current.SetSelectedGameObject(FirstButton);
        Debug.Log("설정창을 엽니다.");
    }

    public void CloseSetting()
    {
        settingPanel.SetActive(false);
        Time.timeScale = 1;
        InputManager.Instance.currentContext = originalContext;
        Debug.Log("설정창을 닫습니다.");
    }

    public void ToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
        Debug.Log("타이틀 화면으로 돌아갑니다.");
    }
}
