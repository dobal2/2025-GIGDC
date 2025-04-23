using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerClickHandler
{
    [Header("버튼 설정")]
    public int buttonLayer = 0; // 버튼 계층
    public bool isDefaultSelected = false; // 시작 시 자동 선택 여부

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        // 시작 버튼으로 지정된 경우 선택 처리
        if (isDefaultSelected)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }

    // 방향키 또는 마우스로 선택되었을 때
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log($"[UIButton] 선택됨: {gameObject.name}");
        PlaySelectEffect();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log($"[UIButton] 선택 해제: {gameObject.name}");
        StopSelectEffect();
    }

    // 마우스가 버튼 위에 올라왔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[UIButton] 마우스 호버: {gameObject.name}");

        // 마우스를 우선시하여 현재 선택을 교체
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

        PlayHoverEffect();
    }

    // 마우스로 클릭했을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[UIButton] 클릭됨: {gameObject.name}");
        PlayClickEffect();
    }

    // 이펙트 훅 (외부 이펙트 매니저가 연동 예정)
    void PlaySelectEffect() => Debug.Log("선택 이펙트 실행 예정");
    void StopSelectEffect() => Debug.Log("선택 해제 이펙트 종료 예정");
    void PlayHoverEffect() => Debug.Log("호버 이펙트 실행 예정");
    void PlayClickEffect() => Debug.Log("클릭 이펙트 실행 예정");
}