using UnityEngine;
using UnityEngine.UI;

public class UIButtonEffect : MonoBehaviour
{
    public Text targetText;
    public Color normalColor = Color.black;
    public Color selectedColor = Color.yellow;
    public Color clickColor = Color.cyan;

    void Awake()
    {
        if (targetText == null)
            targetText = GetComponentInChildren<Text>();
    }

    public void StopSelectEffect() // 이펙트 중지 및 제거
    {
        SetTextColor(normalColor);
    }

    public void PlaySelectEffect() // 선택 이펙트 재생
    {
        if (!isActiveAndEnabled) return;

        SetTextColor(selectedColor);
    }

    public void PlayClickEffect() // 클릭 이펙트 재생
    {
        if (!isActiveAndEnabled) return;

        StopAllCoroutines();
        StartCoroutine(ClickFlash());
    }

    System.Collections.IEnumerator ClickFlash()
    {
        SetTextColor(clickColor);
        yield return new WaitForSeconds(0.15f);
        SetTextColor(normalColor);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        StopSelectEffect();
    }

    void SetTextColor(Color color)
    {
        if (targetText != null)
            targetText.color = color;
    }
}