using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

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


        if (Input.GetKeyDown(keyData.Player.UpKey)) TryMoveTo(upButton);
        if (Input.GetKeyDown(keyData.Player.DownKey)) TryMoveTo(downButton);
        if (Input.GetKeyDown(keyData.Player.LeftKey)) TryMoveTo(leftButton);
        if (Input.GetKeyDown(keyData.Player.RightKey)) TryMoveTo(rightButton);

        if (Input.GetKeyDown(keyData.Player.SelectKey))
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
        StopHoverEffect();
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

    void PlayHoverEffect() //호버이펙트
    {
        SetTextColor(selectedColor);
    }

    void PlayClickEffect()
    {
        if (targetText == null) return;
        StopAllCoroutines();
        StartCoroutine(ClickFlash());
    }

    void StopHoverEffect()
    {
        SetTextColor(normalColor);
    }

    System.Collections.IEnumerator ClickFlash() //클릭이펙트
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