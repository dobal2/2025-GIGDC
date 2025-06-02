using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum InputContext { None, UI, Gameplay }
    public InputContext currentContext = InputContext.UI;

    public KeyData keyData;

    [HideInInspector] public GameObject lastSelectedButton;
    private PlayerController _player;

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

    public void RegisterPlayer(PlayerController player)
    {
        _player = player;
    }

    void Update()
    {
        switch (currentContext)
        {
            case InputContext.UI:
                HandleUIInput();
                break;

            case InputContext.Gameplay:
                HandleGameplayInput();
                break;
        }
    }

    #region UI 입력 처리
    void HandleUIInput()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null && AnyUIKeyPressed())
        {
            if (lastSelectedButton != null && lastSelectedButton.activeInHierarchy)
            {
                Debug.Log("UI 포커스 복구");
                EventSystem.current.SetSelectedGameObject(lastSelectedButton);
                selected = lastSelectedButton;
            }
        }

        if (selected == null) return;

        if (!selected.TryGetComponent(out UIButtonController controller)) return;

        if (Input.GetKeyDown(keyData.Ui.UpKey)) TryMove(controller.upButton);
        else if (Input.GetKeyDown(keyData.Ui.DownKey)) TryMove(controller.downButton);
        else if (Input.GetKeyDown(keyData.Ui.LeftKey)) TryMove(controller.leftButton);
        else if (Input.GetKeyDown(keyData.Ui.RightKey)) TryMove(controller.rightButton);

        if (Input.GetKeyDown(keyData.Ui.SelectKey))
        {
            if (selected.TryGetComponent(out Button button)) button.onClick.Invoke();

            if (controller.nextOnClick != null && controller.nextOnClick.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(controller.nextOnClick);
                lastSelectedButton = controller.nextOnClick;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
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

    bool AnyUIKeyPressed()
    {
        return Input.GetKeyDown(keyData.Ui.UpKey) ||
               Input.GetKeyDown(keyData.Ui.DownKey) ||
               Input.GetKeyDown(keyData.Ui.LeftKey) ||
               Input.GetKeyDown(keyData.Ui.RightKey) ||
               Input.GetKeyDown(keyData.Ui.SelectKey);
    }
    #endregion

    #region 인게임 입력 처리
    void HandleGameplayInput()
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
    }
    #endregion
}