using System.Collections;
using UnityEngine;

public class CounterTutorialStep : TutorialStepBase
{
    private TutorialDescriptionView tutorialDescriptionView;
    private bool isStarted = false;

    public override void Enter()
    {
        TutorialManager.Instance.StartCoroutine(DelayedSkipTutorial1());
    }

    public override void Exit()
    {
        Object.Destroy(tutorialDescriptionView.gameObject);
        PlayerController.Instance.OnCounterTry -= OnCounterTry;
    }

    private void OnCounterTry(bool isSuccesful)
    {
        Debug.Log("OnCounterTry: " + isSuccesful);

        if (!isStarted)
            return;

        if (isSuccesful)
        {
            Object.Destroy(tutorialDescriptionView.gameObject);
            tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
            tutorialDescriptionView.Initialize(TutorialManager.VisualData.CounterTutorial.SuccesSprite);

            Object.FindAnyObjectByType<Monster>()?.SetHealth(60);

            TutorialManager.Instance.StartCoroutine(DelayedSkipTutorial2());
        }
        else
        {
            Object.Destroy(tutorialDescriptionView.gameObject);
            tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
            tutorialDescriptionView.Initialize(TutorialManager.VisualData.CounterTutorial.FailSprite);
        }
    }

    private IEnumerator DelayedSkipTutorial1()
    {
        yield return new WaitForSecondsRealtime(3f);

        PlayerController.Instance.OnCounterTry += OnCounterTry;

        Time.timeScale = 0;
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.CounterTutorial.ProgessSprites[1]);

        yield return new WaitForSecondsRealtime(1.5f);

        Object.Destroy(tutorialDescriptionView.gameObject);
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.CounterTutorial.ProgessSprites[2]);

        yield return new WaitForSecondsRealtime(1.5f);

        isStarted = true;
        Time.timeScale = 1;
    }

    private IEnumerator DelayedSkipTutorial2()
    {
        yield return new WaitForSeconds(1.5f);

        TutorialManager.Instance.SkipTutorial();
    }


    public override void Update()
    {
        PlayerHealth.CurrentHealth = 5;
    }
}