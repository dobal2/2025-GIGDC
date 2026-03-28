using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBrightView : MonoBehaviour
{
    [SerializeField] private Image brightView;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetBrightView(float brightDuration)
    {
        StartCoroutine(PlayBrightView(brightDuration));
    }

    private IEnumerator PlayBrightView(float brightView)
    {
        this.brightView.color = new Color(1, 1, 1, 0);
        this.brightView.DOColor(new Color(1, 1, 1, 1), 0.5f);

        yield return new WaitForSeconds(brightView);

        this.brightView.DOKill();
        this.brightView.DOColor(new Color(1, 1, 1, 0), 0.5f);

        yield return new WaitForSeconds(.5f);

        Destroy(gameObject);
    }
}
