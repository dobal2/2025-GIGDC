using UnityEngine;
using UnityEngine.SceneManagement;
using static InputManager;

public class SettingWindowManager : MonoBehaviour
{
    public static SettingWindowManager Instance { get; private set; }

    [SerializeField] private GameObject settingWindow;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject keybindingPanel;
    [SerializeField] private GameObject weaponsInfoPanel;

    private InputContext originalContext;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        originalContext = InputManager.Instance.CurrentContext;
        settingWindow.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        ViewAudioPanel();
    }

    private void OnDisable() => SceneManager.activeSceneChanged -= OnActiveSceneChanged;

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene) => originalContext = InputManager.Instance.CurrentContext;

    public void OpenSetting()
    {
        settingWindow.SetActive(true);
        ViewAudioPanel();
        if (SceneManager.GetActiveScene().name != "TitleScene") Time.timeScale = 0;
        originalContext = InputManager.Instance.CurrentContext;
        InputManager.Instance.CurrentContext = InputContext.UI;
        Debug.Log("설정창을 엽니다.");
    }

    public void CloseSetting()
    {
        Time.timeScale = 1;
        InputManager.Instance.CurrentContext = originalContext;
        Debug.Log("설정창을 닫습니다.");
        settingWindow.SetActive(false);
    }

    public void ToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
        Debug.Log("타이틀 화면으로 돌아갑니다.");
    }

    public void ViewAudioPanel()
    {
        audioPanel.SetActive(true);
        keybindingPanel.SetActive(false);
        weaponsInfoPanel.SetActive(false);
    }

    public void ViewKeybindingPanel()
    {
        audioPanel.SetActive(false);
        keybindingPanel.SetActive(true);
        weaponsInfoPanel.SetActive(false);
    }

    public void ViewWeaponsInfoPanel()
    {
        audioPanel.SetActive(false);
        keybindingPanel.SetActive(false);
        weaponsInfoPanel.SetActive(true);
    }
}
