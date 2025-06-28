using System.Collections;
using TMPro;
using UnityEngine;

public class UIButtonEffect : MonoBehaviour
{
    public TMP_Text targetText;
    public Color normalColor;
    public Color hoverColer;
    public float clickFlashInterval = 0.05f;

    bool isHovering = false;
    bool isClickEffectPlaying = false;

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

    void OnDisable()
    {
        if (clickEffectCoroutine != null)
        {
            StopCoroutine(clickEffectCoroutine);
            clickEffectCoroutine = null;
        }

        isClickEffectPlaying = false;
        isHovering = false;

        targetText.faceColor = normalColor;
    }

    public void StartHoverEffect() => isHovering = true; // 호버이펙트 시작
    public void StopHoverEffect() => isHovering = false; // 호버이펙트 종료
    public void PlayClickEffect() // 클릭 이펙트 (자동종료)
    {
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