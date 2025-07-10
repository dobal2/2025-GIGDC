using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Image tutorial1;
    [SerializeField] private Image tutorial2;
    [SerializeField] private Image tutorial3;
    [SerializeField] private Image tutorial4;
    [SerializeField] private Image tutorial5;
    [SerializeField] private Image tutorial6;
    [SerializeField] private Image tutorial7;
    [SerializeField] private Image tutorial8;
    [SerializeField] private Image tutorial9;
    [SerializeField] private Image tutorial10;

    private int indexProgress = 0;

    private void Update()
    {
        switch(indexProgress)
        {
            case 0:
                Tutorial1();
                break;
            case 1:
                Tutorial2(); 
                break;
            case 2:
                Tutorial3();
                break;
            case 3:
                Tutorial4();
                break;
            case 4:
                Tutorial5();
                break;
            case 5:
                Tutorial6();
                break;
            case 6:
                Tutorial7();
                break;
            case 7:
                Tutorial8();
                break;
            case 8:
                Tutorial9();
                break;
            case 9:
                StartCoroutine(Tutorial10());
                break;
        }
    }

    private void Fade(Image fadeTarget) =>
        fadeTarget.DOFade(0, 0.5f);

    private void Tutorial1()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Fade(tutorial1);
            indexProgress++;
        }
            
    }

    private void Tutorial2()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            Fade(tutorial2);
            indexProgress++;
        }
    }

    private void Tutorial3()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Fade(tutorial3);
            indexProgress++;
        }
    }

    private void Tutorial4()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Fade(tutorial4);
            indexProgress++;
        }
    }

    private void Tutorial5()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Fade(tutorial5);
            indexProgress++;
        }
    }

    private void Tutorial6()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Fade(tutorial6);
            indexProgress++;
        }
    }

    private void Tutorial7()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Fade(tutorial7);
            indexProgress++;
        }
    }

    private void Tutorial8()
    {
        StartCoroutine(ProgressIndex());
    }

    private void Tutorial9()
    {
        Fade(tutorial8);
        StartCoroutine(ProgressIndex());
    }

    private IEnumerator Tutorial10()
    {
        Fade(tutorial9);
        yield return new WaitForSeconds(3f);
        Stage.Progress();
        SceneController.Instance.LoadScene(SceneController.SceneType.Lobby_Over);
    }

    private IEnumerator ProgressIndex()
    {
        yield return new WaitForSeconds(1.5f);
        indexProgress++;
    }
}
