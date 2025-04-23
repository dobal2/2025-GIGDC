using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerClickHandler
{
    [Header("버튼 설정")]
    public bool isDefaultSelected = false;

    [Header("이웃 버튼")]
    public GameObject upButton;
    public GameObject downButton;
    public GameObject leftButton;
    public GameObject rightButton;

    [Header("클릭 후 다음 선택 버튼")]
    public GameObject nextOnClick;


    [Header("텍스트 색상 효과")]
    public Text targetText;
    public Color normalColor = Color.black;
    public Color selectedColor = Color.yellow;
    public Color clickColor = Color.cyan;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (targetText == null)
        {
            targetText = GetComponentInChildren<Text>();
        }
    }

    void Start()
    {
        if (isDefaultSelected)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null || selected != this.gameObject) return;


        if (Input.GetKeyDown(KeyCode.UpArrow)) TryMoveTo(upButton);
        if (Input.GetKeyDown(KeyCode.DownArrow)) TryMoveTo(downButton);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMoveTo(leftButton);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryMoveTo(rightButton);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryMoveTo(nextOnClick);
            button.onClick.Invoke();
            TryMoveTo(nextOnClick);
        }
    }

    void TryMoveTo(GameObject target)
    {
        if (target != null && target.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(target);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHoverEffect();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StopSelectEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

        PlayHoverEffect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClickEffect();
    }

    void PlayHoverEffect() => SetTextColor(selectedColor);
    void StopSelectEffect() => SetTextColor(normalColor);
    void PlayClickEffect()
    {
        if (targetText == null) return;
        StopAllCoroutines();
        StartCoroutine(ClickFlash());
    }

    System.Collections.IEnumerator ClickFlash()
    {
        SetTextColor(clickColor);
        yield return new WaitForSeconds(0.15f);
        SetTextColor(selectedColor);
    }

    void SetTextColor(Color color)
    {
        if (targetText != null) targetText.color = color;
    }
}