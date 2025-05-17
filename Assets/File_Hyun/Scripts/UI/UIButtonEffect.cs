using UnityEngine;
using TMPro;

public class UIButtonEffect : MonoBehaviour
{
    public TMP_Text targetText;
    public Color normalColor = Color.black;
    public Color selectedColor = Color.yellow;

    public void StopSelectEffect() // 이펙트 중지 및 제거
    {
        SetTextColor(normalColor);
    }

    public void PlaySelectEffect() // 선택 이펙트 재생
    {
        if (!isActiveAndEnabled) return;

        SetTextColor(selectedColor);
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