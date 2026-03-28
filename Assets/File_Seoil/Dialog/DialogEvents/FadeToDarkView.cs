using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeToDarkView : MonoBehaviour
{
    [SerializeField] private Image darkView;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetDarkView(float darkDuration)
    {
        StartCoroutine(PlayDarkView(darkDuration));
    }

    private IEnumerator PlayDarkView(float darkDuration)
    {
        darkView.color = new Color(0, 0, 0, 0);
        darkView.DOColor(new Color(0, 0, 0, 1), 0.5f);

        yield return new WaitForSeconds(darkDuration);

        darkView.DOKill();
        darkView.DOColor(new Color(0, 0, 0, 0), 0.5f);

        yield return new WaitForSeconds(.5f);

        Destroy(gameObject);
    }
}
