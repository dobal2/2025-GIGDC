using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum InputContext { None, UI, Gameplay }
    public InputContext currentContext = InputContext.UI;

    public KeyData keyData;
    public GameObject lastSelectedButton;

    public PlayerController player;

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

        var controller = selected.GetComponent<UIButtonController>();
        if (controller == null) return;

        if (Input.GetKeyDown(keyData.Ui.UpKey)) TryMove(controller.upButton);
        else if (Input.GetKeyDown(keyData.Ui.DownKey)) TryMove(controller.downButton);
        else if (Input.GetKeyDown(keyData.Ui.LeftKey)) TryMove(controller.leftButton);
        else if (Input.GetKeyDown(keyData.Ui.RightKey)) TryMove(controller.rightButton);

        if (Input.GetKeyDown(keyData.Ui.SelectKey))
        {
            var btn = selected.GetComponent<Button>();
            if (btn != null) btn.onClick.Invoke();

            controller.onClick?.Invoke();

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
        //player.MoveLeft = Input.GetKey(keyData.Player.LeftMoveKey);
        //player.MoveRight = Input.GetKey(keyData.Player.RightMoveKey);
        //player.IsRunning = Input.GetKey(keyData.Player.RunKey);
        //player.JumpPressed = Input.GetKeyDown(keyData.Player.JumpKey);
        //player.AttackPressed = Input.GetKeyDown(keyData.Player.AttackKey);

        //// 무기 전환
        //if (Input.GetKeyDown(keyData.Player.ItemSelectionLeftKey))
        //    WeaponManager.Instance.SelectLeft();

        //if (Input.GetKeyDown(keyData.Player.ItemSelectionRightKey))
        //    WeaponManager.Instance.SelectRight();
    }
    #endregion
}