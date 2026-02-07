using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static InputManager;

public class SettingWindow : MonoBehaviour
{
    public static SettingWindow Instance { get; private set; }

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject Buttons;
    [SerializeField] private GameObject[] Windows;
    [SerializeField] private GameObject NextOnClick;

    InputContext originalContext;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Initialize();
        settingPanel.SetActive(false);
        originalContext = InputManager.Instance.CurrentContext;
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    void Initialize()
    {
        Buttons.SetActive(true);
        foreach (GameObject window in Windows)
            window.SetActive(false);

    }

    void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        originalContext = InputManager.Instance.CurrentContext;
        settingPanel.SetActive(false);
    }

    public void OpenSetting()
    {
        settingPanel.SetActive(true);
        Initialize();
        if (SceneManager.GetActiveScene().name != "TitleScene") Time.timeScale = 0;
        originalContext = InputManager.Instance.CurrentContext;
        InputManager.Instance.CurrentContext = InputContext.UI;
        Debug.Log("설정창을 엽니다.");
    }

    public void CloseSetting()
    {
        settingPanel.SetActive(false);
        Time.timeScale = 1;
        InputManager.Instance.CurrentContext = originalContext;
        if (SceneManager.GetActiveScene().name == "TitleScene") EventSystem.current.SetSelectedGameObject(NextOnClick);
        Debug.Log("설정창을 닫습니다.");
    }

    public void ToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
        Debug.Log("타이틀 화면으로 돌아갑니다.");
    }
}
