using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingWindow : MonoBehaviour
{
    public static SettingWindow Instance { get; private set; }

    [SerializeField] private GameObject settingPanel;

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
        Debug.Log("설정창을 엽니다.");
    }

    public void CloseSetting()
    {
        settingPanel.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("설정창을 닫습니다.");
    }

    public void ToTitle()
    {
        settingPanel.SetActive(false);
        SceneManager.LoadScene("TitleScreen");
        Debug.Log("타이틀 화면으로 돌아갑니다.");
    }
}
