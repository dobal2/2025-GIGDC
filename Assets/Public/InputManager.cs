using System;
using UnityEngine;
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

    public event Action OnSelectKeyDown;
    public event Action OnUILeftKeyDown;
    public event Action OnUIRightKeyDown;
    public event Action OnUIUpKeyDown;
    public event Action OnUIDownKeyDown;

    [HideInInspector] public GameObject lastSelectedButton;
    [HideInInspector] public bool IsCapturingKey = false;
    private PlayerController _player;
    private LobbyPlayerController _LobbyPlayer;
    private DialogGenerator _dialogGenerator;

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

    #region UI 입력 처리
    void HandlePause()
    {
        if (Input.GetKeyDown(keyData.UI.PauseKey))
        {
            if (CurrentContext == InputContext.UI)
                SettingWindowManager.Instance.CloseSetting();
            else if (SceneManager.GetActiveScene().name != "TitleScene")
                SettingWindowManager.Instance.OpenSetting();
        }
    }

    void HandleUIInput()
    {
        if (Input.GetKeyDown(keyData.UI.LeftKey))
            OnUILeftKeyDown?.Invoke();

        if (Input.GetKeyDown(keyData.UI.RightKey))
            OnUIRightKeyDown?.Invoke();

        if (Input.GetKeyDown(keyData.UI.UpKey))
            OnUIUpKeyDown?.Invoke();

        if (Input.GetKeyDown(keyData.UI.DownKey))
            OnUIDownKeyDown?.Invoke();

        if (Input.GetKeyDown(keyData.UI.SelectKey))
            OnSelectKeyDown?.Invoke();
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