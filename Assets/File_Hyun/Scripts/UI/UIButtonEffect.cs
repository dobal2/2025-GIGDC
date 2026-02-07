using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonEffect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColer;
    [SerializeField] private float clickFlashInterval = 0.05f;
    [SerializeField] private Button button;
    [SerializeField] private AudioSource click;

    private bool isHovering = false;
    private bool isClickEffectPlaying = false;

    Coroutine clickEffectCoroutine;

    void Update()
    {
        if (!isClickEffectPlaying)
        {
            if (isHovering)
                targetText.color = hoverColer;
            else
                targetText.color = normalColor;
        }
    }

    private void OnEnable() => button.onClick.AddListener(PlayClickEffect);

    void OnDisable()
    {
        button.onClick.RemoveListener(PlayClickEffect);

        if (clickEffectCoroutine != null)
        {
            StopCoroutine(clickEffectCoroutine);
            clickEffectCoroutine = null;
        }

        isClickEffectPlaying = false;
        isHovering = false;

        targetText.faceColor = normalColor;
    }

    public void OnSelect(BaseEventData eventData) => StartHoverEffect();
    public void OnDeselect(BaseEventData eventData) => StopHoverEffect();
    

    public void StartHoverEffect() => isHovering = true; // 호버이펙트 시작
    public void StopHoverEffect() => isHovering = false; // 호버이펙트 종료
    public void PlayClickEffect() // 클릭 이펙트 (자동종료)
    {
        click.Play();

        if (clickEffectCoroutine != null)
            StopCoroutine(clickEffectCoroutine);

        clickEffectCoroutine = StartCoroutine(ClickFlashEffect());
    }

    IEnumerator ClickFlashEffect()
    {
        isClickEffectPlaying = true;

        targetText.color = normalColor;
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = hoverColer;
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = normalColor;
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = hoverColer;
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = normalColor;
        isClickEffectPlaying = false;
    }
}