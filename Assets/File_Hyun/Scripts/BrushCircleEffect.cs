using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BrushCircleEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("이미지")]
    public Image brushImage;
    public Sprite[] brushSprites;

    [Header("속도")]
    public float totalDuration = 0.15f;

    Coroutine drawCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (brushSprites.Length > 0)
        {
            int randIndex = Random.Range(0, brushSprites.Length);
            brushImage.sprite = brushSprites[randIndex];
        }

        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        drawCoroutine = StartCoroutine(DrawBrushCircle());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        brushImage.fillAmount = 0;
        brushImage.enabled = false;
    }

    IEnumerator DrawBrushCircle()
    {
        brushImage.enabled = true;
        brushImage.fillAmount = 0f;

        float[] checkpoints = { 0f, 0.2f, 0.4f, 0.7f, 0.9f, 1f };
        float[] speeds = {
            totalDuration * 0.25f,
            totalDuration * 0.10f,
            totalDuration * 0.30f,
            totalDuration * 0.10f,
            totalDuration * 0.25f
        };

        for (int i = 0; i < checkpoints.Length - 1; i++)
        {
            float from = checkpoints[i];
            float to = checkpoints[i + 1];
            float segmentTime = speeds[i];
            float t = 0f;

            while (t < segmentTime)
            {
                t += Time.deltaTime;
                float normalized = t / segmentTime;
                brushImage.fillAmount = Mathf.Lerp(from, to, normalized);
                yield return null;
            }

            brushImage.fillAmount = to;
        }
    }
}