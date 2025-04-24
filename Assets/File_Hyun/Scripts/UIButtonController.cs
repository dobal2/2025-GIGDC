using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerClickHandler
{
    [Header("초기 선택 여부")]
    public bool isDefaultSelected = false;

    [Header("이웃 버튼 지정")]
    public GameObject upButton;
    public GameObject downButton;
    public GameObject leftButton;
    public GameObject rightButton;

    [Header("클릭 후 선택될 버튼")]
    public GameObject nextOnClick;

    [Header("이펙트")]
    public UnityEvent onSelect;
    public UnityEvent onDeselect;
    public UnityEvent onClick;

    void Start()
    {
        if (isDefaultSelected)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }

    // 현재 선택된 방향에 해당하는 이웃 버튼 반환
    public GameObject GetNeighbor(Vector2 direction)
    {
        if (direction == Vector2.up) return upButton;
        if (direction == Vector2.down) return downButton;
        if (direction == Vector2.left) return leftButton;
        if (direction == Vector2.right) return rightButton;
        return null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        InputManager.Instance.lastSelectedButton = this.gameObject;
        onSelect?.Invoke();
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
        if (nextOnClick != null && nextOnClick.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(nextOnClick);
        }
    }
}