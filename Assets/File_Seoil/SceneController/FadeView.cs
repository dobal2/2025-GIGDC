using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeView : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(FadeColor());
    }

    private IEnumerator FadeColor()
    {
        fadeImage.DOColor(new Color(0, 0, 0, 1), 0.5f);
        yield return new WaitForSeconds(0.5f);
        fadeImage.DOKill();
        fadeImage.DOColor(new Color(0, 0, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);

    }
}
