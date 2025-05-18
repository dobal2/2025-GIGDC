using UnityEngine;
using TMPro;
using System.Collections;

public class UIButtonEffect : MonoBehaviour
{
    public TMP_Text targetText;
    public Color normalColor;
    public Color gradientColor1;
    public Color gradientColor2;
    public float gradientSpeed = 1.5f;
    public float clickFlashInterval = 0.05f;

    bool isHovering = false;
    bool isClickEffectPlaying = false;

    float gradientT = 0f;
    bool gradientForward = true;

    Coroutine clickEffectCoroutine;

    void Update()
    {
        if (!isClickEffectPlaying)
        {
            if (isHovering)
                targetText.color = Color.Lerp(gradientColor1, gradientColor2, gradientT);
            else
                targetText.color = normalColor;
        }

        UpdateGradientT();
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

    void UpdateGradientT()
    {
        if (!isHovering && !isClickEffectPlaying)
            return;

        float delta = Time.deltaTime * gradientSpeed;
        gradientT += gradientForward ? delta : -delta;

        if (gradientT >= 1f)
        {
            gradientT = 1f;
            gradientForward = false;
        }
        else if (gradientT <= 0f)
        {
            gradientT = 0f;
            gradientForward = true;
        }
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

        targetText.color = Color.Lerp(gradientColor1, gradientColor2, gradientT);
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = normalColor;
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = Color.Lerp(gradientColor1, gradientColor2, gradientT);
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = normalColor;
        isClickEffectPlaying = false;
    }
}