using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum InputContext { None, UI, Gameplay, Lobby, Dialog }

    private InputContext _context = InputContext.UI;
    public InputContext CurrentContext
    {
        get => _context;
        set
        {
            if (_context != value)
            {
                _context = value;
                ResetInput();
            }
        }
    }

    public KeyData keyData;
    public AudioSource Click;

    public static event Action OnUILeftKey;
    public static event Action OnUIRightKey;

    [HideInInspector] public GameObject lastSelectedButton;
    [HideInInspector] public bool IsCapturingKey = false;
    private PlayerController _player;
    private LobbyPlayerController _LobbyPlayer;
    private DialogGenerator _dialogGenerator;

    private float _lastClickTime = -999f;
    [SerializeField] private float ClickCooldown = 0.3f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

#if !UNITY_EDITOR
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
#endif
    }

    void Update()
    {
        switch (CurrentContext)
        {
            case InputContext.UI:
                HandleUIInput();
                break;

            case InputContext.Gameplay:
                HandlePlayerInput();
                break;

            case InputContext.Lobby:
                HandleLobbyInput();
                break;

            case InputContext.Dialog:
                HandleDialogInput();
                break;
            case InputContext.None:
                break;
        }
        HandlePause();
    }

    void HandlePause()
    {
        if (Input.GetKeyDown(keyData.UI.PauseKey))
        {
            if (CurrentContext == InputContext.UI)
                SettingWindow.Instance.CloseSetting();
            else if (SceneManager.GetActiveScene().name != "TitleScene")
                SettingWindow.Instance.OpenSetting();
        }
    }

    #region UI 입력 처리
    void HandleUIInput()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null)
        {
            if (lastSelectedButton != null && lastSelectedButton.activeInHierarchy)
            {
                Debug.Log("UI 포커스 복구");
                EventSystem.current.SetSelectedGameObject(lastSelectedButton);
                selected = lastSelectedButton;
            }
        }

        if (selected == null) return;
        if (IsCapturingKey) return;

        if (!selected.TryGetComponent(out UIButtonController controller))
        {
            Debug.LogWarning("선택된 GameObject가 UIButtonController를 포함하지 않습니다: " + selected.name);
            return;
        }

        if (Input.GetKeyDown(keyData.UI.UpKey)) TryMove(controller.upButton);
        else if (Input.GetKeyDown(keyData.UI.DownKey)) TryMove(controller.downButton);
        else if (Input.GetKeyDown(keyData.UI.LeftKey))
        {
            TryMove(controller.leftButton);
            OnUILeftKey?.Invoke();
        }
        else if (Input.GetKeyDown(keyData.UI.RightKey))
        {
            TryMove(controller.rightButton);
            OnUIRightKey?.Invoke();
        }

        if (Input.GetKeyDown(keyData.UI.SelectKey))
        {
            if (Time.unscaledTime - _lastClickTime < ClickCooldown)
                return;

            _lastClickTime = Time.unscaledTime;
            Click.Play();

            if (selected.TryGetComponent(out Button button))
                button.onClick.Invoke();

            if (controller.nextOnClick != null && controller.nextOnClick.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(controller.nextOnClick);
                lastSelectedButton = controller.nextOnClick;
            }
        }

        if (selected != lastSelectedButton)
            lastSelectedButton = selected;
    }

    void TryMove(GameObject target)
    {
        if (target != null && target.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(target);
            lastSelectedButton = target;
        }
    }
    #endregion

    #region 인게임 입력 처리
    void HandlePlayerInput()
    {
        if (_player == null) return;

        float horizontal = 0f;
        if (Input.GetKey(keyData.Player.LeftMoveKey)) horizontal -= 1f;
        if (Input.GetKey(keyData.Player.RightMoveKey)) horizontal += 1f;
        _player.MoveInput = horizontal;

        _player.JumpPressed = Input.GetKeyDown(keyData.Player.JumpKey);
        _player.JumpHeld = Input.GetKey(keyData.Player.JumpKey);
        if (Input.GetKeyUp(keyData.Player.JumpKey)) PlayerController.Instance.StopRising();
        _player.DashPressed = Input.GetKeyDown(keyData.Player.DashKey);
        _player.DownHeld = Input.GetKey(keyData.Player.DownMoveKey);

        _player.AttackPressed = Input.GetKeyDown(keyData.Player.AttackKey);
        _player.SkillPressed = Input.GetKeyDown(keyData.Player.SkillKey);
        _player.SkillHeld = Input.GetKey(keyData.Player.SkillKey);
        _player.ChangePressed = Input.GetKeyDown(keyData.Player.WeaponchangeKey);
    }

    public void RegisterPlayer(PlayerController player) => _player = player;
    #endregion

    #region 로비 입력 처리
    void HandleLobbyInput()
    {
        if (_LobbyPlayer == null) return;

        float horizontal = 0f;
        if (Input.GetKey(keyData.Player.LeftMoveKey)) horizontal -= 1f;
        if (Input.GetKey(keyData.Player.RightMoveKey)) horizontal += 1f;
        _LobbyPlayer.MoveInput = horizontal;
    }

    public void RegisterLobby(LobbyPlayerController lobbyplayer) => _LobbyPlayer = lobbyplayer;
    #endregion

    #region 대사 입력 처리
    void HandleDialogInput()
    {
        if (_dialogGenerator == null) return;

        if (Input.GetKeyDown(keyData.Player.ProcessKey))
            _dialogGenerator.ProcessDialog();
    }

    public void RegisterDialog(DialogGenerator dialog) => _dialogGenerator = dialog;
    #endregion

    void ResetInput()
    {
        if (_player != null)
        {
            _player.MoveInput = 0f;
            _player.JumpPressed = false;
            _player.JumpHeld = false;
            _player.DashPressed = false;
            _player.AttackPressed = false;
            _player.SkillPressed = false;
            _player.SkillHeld = false;
            _player.ChangePressed = false;
            _player.DownHeld = false;
        }

        if (_LobbyPlayer != null)
        {
            _LobbyPlayer.MoveInput = 0f;
        }
    }
}