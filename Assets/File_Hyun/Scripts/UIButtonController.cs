using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
public class UIButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerClickHandler
{
    public KeyData keyData;

    [Header("시작 버튼 설정")]
    public bool isDefaultSelected = false;

    [Header("이웃 버튼")]
    public GameObject upButton;
    public GameObject downButton;
    public GameObject leftButton;
    public GameObject rightButton;

    [Header("클릭 후 다음 선택 버튼")]
    public GameObject nextOnClick;

    [Header("이벤트 연결")]
    public UnityEvent onSelect;
    public UnityEvent onDeselect;
    public UnityEvent onClick;

    private Button button;
    private static GameObject lastSelectedButton;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        if (isDefaultSelected)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

        // 기본 선택된 버튼이 자신이라면 저장
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            lastSelectedButton = this.gameObject;
        }
    }

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        // 선택된 버튼이 없고, 키 입력이 발생한 경우 → 마지막 버튼으로 복원
        if (selected == null && (
            Input.GetKeyDown(keyData.Player.UpKey) ||
            Input.GetKeyDown(keyData.Player.DownKey) ||
            Input.GetKeyDown(keyData.Player.LeftKey) ||
            Input.GetKeyDown(keyData.Player.RightKey) ||
            Input.GetKeyDown(keyData.Player.SelectKey)))
        {
            if (lastSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelectedButton);
                return;
            }
        }

        if (selected == null || selected != this.gameObject) return;

        if (Input.GetKeyDown(keyData.Player.UpKey)) TryMoveTo(upButton);
        if (Input.GetKeyDown(keyData.Player.DownKey)) TryMoveTo(downButton);
        if (Input.GetKeyDown(keyData.Player.LeftKey)) TryMoveTo(leftButton);
        if (Input.GetKeyDown(keyData.Player.RightKey)) TryMoveTo(rightButton);

        if (Input.GetKeyDown(keyData.Player.SelectKey))
        {
            button.onClick.Invoke();
            onClick?.Invoke();
            TryMoveTo(nextOnClick);
        }
    }

    void TryMoveTo(GameObject target)
    {
        if (target != null && target.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(target);
            lastSelectedButton = target;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelect?.Invoke();
        lastSelectedButton = this.gameObject;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        onDeselect?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

        onSelect?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
        TryMoveTo(nextOnClick);
    }
}