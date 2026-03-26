using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private Button button;
    [SerializeField] private AudioSource click;

    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private float normalTextScale = 0.7f;
    [SerializeField] private float hoverTextScale = 1f;
    [SerializeField] private float transitionDuration = 0.3f;

    [SerializeField] private Image blueDotPrefab;
    [SerializeField] private RectTransform dotsParent;
    [SerializeField] private float dotSpacingFromText = 20f;
    [SerializeField] private float dotYOffset = 0f;
    [SerializeField] private float dotScale = 0.1f;

    [SerializeField] private float clickFlashInterval = 0.05f;

    private bool isHovering;
    private bool isClickEffectPlaying;

    private Coroutine clickEffectCoroutine;

    private Image leftDot;
    private Image rightDot;

    public void OnEnable()
    {
        button.onClick.AddListener(PlayClickEffect);
        ApplyNormalStateImmediate();
    }

    public void OnDisable()
    {
        button.onClick.RemoveListener(PlayClickEffect);

        if (clickEffectCoroutine != null)
            StopCoroutine(clickEffectCoroutine);

        KillTextTweens();
        RemoveDotsImmediate();

        isHovering = false;
        isClickEffectPlaying = false;

        targetText.color = normalColor;
        targetText.rectTransform.localScale = Vector3.one * normalTextScale;
    }

    public void OnPointerEnter(PointerEventData eventData) => StartHoverEffect();

    public void OnPointerExit(PointerEventData eventData) => StopHoverEffect();

    public void StartHoverEffect()
    {
        isHovering = true;
        PlayHoverState();
    }

    public void StopHoverEffect()
    {
        isHovering = false;
        PlayNormalState();
    }

    public void PlayClickEffect()
    {
        click.Play();

        if (clickEffectCoroutine != null)
            StopCoroutine(clickEffectCoroutine);

        clickEffectCoroutine = StartCoroutine(ClickFlashEffect());
    }

    private IEnumerator ClickFlashEffect()
    {
        isClickEffectPlaying = true;

        DOTween.Kill(targetText);

        targetText.color = normalColor;
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = hoverColor;
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = normalColor;
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = hoverColor;
        yield return new WaitForSeconds(clickFlashInterval);

        targetText.color = isHovering ? hoverColor : normalColor;
        isClickEffectPlaying = false;
        clickEffectCoroutine = null;
    }

    private void PlayHoverState()
    {
        KillTextTweens();

        targetText.rectTransform
            .DOScale(Vector3.one * hoverTextScale, transitionDuration)
            .SetEase(Ease.OutQuad);

        if (!isClickEffectPlaying)
        {
            targetText
                .DOColor(hoverColor, transitionDuration)
                .SetEase(Ease.OutQuad);
        }

        CreateOrRefreshDots();
        AnimateDotsIn();
    }

    private void PlayNormalState()
    {
        KillTextTweens();

        targetText.rectTransform
            .DOScale(Vector3.one * normalTextScale, transitionDuration)
            .SetEase(Ease.OutQuad);

        if (!isClickEffectPlaying)
        {
            targetText
                .DOColor(normalColor, transitionDuration)
                .SetEase(Ease.OutQuad);
        }

        AnimateDotsOut();
    }

    private void ApplyNormalStateImmediate()
    {
        isHovering = false;
        isClickEffectPlaying = false;

        KillTextTweens();
        RemoveDotsImmediate();

        targetText.color = normalColor;
        targetText.rectTransform.localScale = Vector3.one * normalTextScale;
    }

    private void KillTextTweens()
    {
        DOTween.Kill(targetText);
        DOTween.Kill(targetText.rectTransform);
    }

    private void CreateOrRefreshDots()
    {
        if (leftDot == null)
            leftDot = CreateDot("LeftDot");

        if (rightDot == null)
            rightDot = CreateDot("RightDot");
    }

    private Image CreateDot(string objectName)
    {
        Image dot = Instantiate(blueDotPrefab, dotsParent);
        RectTransform dotRect = dot.rectTransform;
        Vector2 startPosition = GetDotStartPosition();

        dot.name = objectName;
        dot.raycastTarget = false;
        dotRect.anchoredPosition = startPosition;
        dotRect.localScale = Vector3.zero;

        Color color = dot.color;
        color.a = 0f;
        dot.color = color;

        return dot;
    }

    private void AnimateDotsIn()
    {
        GetDotTargetPositions(out Vector2 leftTargetPosition, out Vector2 rightTargetPosition);

        AnimateDotIn(leftDot, leftTargetPosition);
        AnimateDotIn(rightDot, rightTargetPosition);
    }

    private void AnimateDotIn(Image dot, Vector2 targetPosition)
    {
        RectTransform dotRect = dot.rectTransform;

        DOTween.Kill(dot);
        DOTween.Kill(dotRect);

        dotRect
            .DOAnchorPos(targetPosition, transitionDuration)
            .SetEase(Ease.OutQuad);

        dotRect
            .DOScale(Vector3.one * dotScale, transitionDuration)
            .SetEase(Ease.OutQuad);

        dot
            .DOFade(1f, transitionDuration)
            .SetEase(Ease.OutQuad);
    }

    private void AnimateDotsOut()
    {
        AnimateDotOut(leftDot);
        AnimateDotOut(rightDot);
    }

    private void AnimateDotOut(Image dot)
    {
        if (dot == null)
            return;

        RectTransform dotRect = dot.rectTransform;
        Vector2 startPosition = GetDotStartPosition();

        DOTween.Kill(dot);
        DOTween.Kill(dotRect);

        dotRect
            .DOAnchorPos(startPosition, transitionDuration)
            .SetEase(Ease.OutQuad);

        dotRect
            .DOScale(Vector3.zero, transitionDuration)
            .SetEase(Ease.OutQuad);

        dot
            .DOFade(0f, transitionDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => DestroyDot(dot));
    }

    private void DestroyDot(Image dot)
    {
        if (dot == leftDot)
            leftDot = null;

        if (dot == rightDot)
            rightDot = null;

        Destroy(dot.gameObject);
    }

    private void RemoveDotsImmediate()
    {
        if (leftDot != null)
            Destroy(leftDot.gameObject);

        if (rightDot != null)
            Destroy(rightDot.gameObject);

        leftDot = null;
        rightDot = null;
    }

    private void GetDotTargetPositions(out Vector2 leftPosition, out Vector2 rightPosition)
    {
        targetText.ForceMeshUpdate();

        Vector2 textPosition = targetText.rectTransform.anchoredPosition;
        float halfWidth = targetText.preferredWidth * hoverTextScale * 0.5f;
        float xOffset = halfWidth + dotSpacingFromText;

        leftPosition = new Vector2(textPosition.x - xOffset, textPosition.y + dotYOffset);
        rightPosition = new Vector2(textPosition.x + xOffset, textPosition.y + dotYOffset);
    }

    private Vector2 GetDotStartPosition()
    {
        Vector2 textPosition = targetText.rectTransform.anchoredPosition;
        return new Vector2(textPosition.x, textPosition.y + dotYOffset);
    }
}