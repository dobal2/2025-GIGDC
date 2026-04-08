using System.Collections;
using UnityEngine;

public class CombatTutorialStep : TutorialStepBase
{
    private TutorialDescriptionView tutorialDescriptionView;

    public override void Enter()
    {
        TutorialManager.Instance.StartCoroutine(ProcessTutorial());
        StageManager.OnAllObjectKilled += OnAllObjectKilled;
    }

    private IEnumerator ProcessTutorial()
    {
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.CombatTutorial.ProgessSprites[0]);

        yield return new WaitForSeconds(1.5f);

        if(tutorialDescriptionView != null) Object.Destroy(tutorialDescriptionView.gameObject);
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.CombatTutorial.ProgessSprites[1]);
    }

    private void OnAllObjectKilled()
    {
        StageManager.OnAllObjectKilled -= OnAllObjectKilled;
        if (tutorialDescriptionView != null)
            Object.Destroy(tutorialDescriptionView.gameObject);
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.CombatTutorial.ProgessSprites[2]);

        TutorialManager.Instance.StartCoroutine(DelayedSkipTutorial());
    }

    private IEnumerator DelayedSkipTutorial()
    {
        yield return new WaitForSeconds(1.5f);
        TutorialManager.Instance.SkipTutorial();
    }

    public override void Exit()
    {
        StageManager.OnAllObjectKilled -= OnAllObjectKilled;
    }

    public override void Update()
    {

    }
}