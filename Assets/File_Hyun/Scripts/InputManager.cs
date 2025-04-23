using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum InputContext { None, UI, Gameplay }
    public InputContext currentContext = InputContext.UI;

    public KeyData keyData;

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
        }
    }

    #region UI 입력 처리
    void HandleUIInput()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        var controller = selected.GetComponent<UIButtonController>();
        if (controller == null) return;

        if (Input.GetKeyDown(keyData.Player.UpKey)) TryMove(controller.upButton);
        else if (Input.GetKeyDown(keyData.Player.DownKey)) TryMove(controller.downButton);
        else if (Input.GetKeyDown(keyData.Player.LeftKey)) TryMove(controller.leftButton);
        else if (Input.GetKeyDown(keyData.Player.RightKey)) TryMove(controller.rightButton);

        if (Input.GetKeyDown(keyData.Player.SelectKey))
        {
            var btn = selected.GetComponent<Button>();
            if (btn != null) btn.onClick.Invoke();

            controller.onClick?.Invoke();
            TryMove(controller.nextOnClick);
        }
    }

    void TryMove(GameObject target)
    {
        if (target != null && target.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(target);
        }
    }
    #endregion
}