using System.Collections;
using UnityEngine;

public class SkillAttackTutorialStep : TutorialStepBase
{
    private TutorialDescriptionView tutorialDescriptionView;
    private Coroutine skipTutorialCoroutine = null;
    private int index = 0;

    public override void Enter()
    {
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.SkillAttackTutorial.ProgessSprites[index]);

        PlayerController.Instance.RestoreEnergy(40);
    }

    public override void Exit()
    {
        Object.Destroy(tutorialDescriptionView.gameObject);
        StageManager.Instance.Clear();
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            index++;
            if(index <= 1)
            {
                Object.Destroy(tutorialDescriptionView.gameObject);
                tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
                tutorialDescriptionView.Initialize(TutorialManager.VisualData.SkillAttackTutorial.ProgessSprites[index]);
            }
            else if (skipTutorialCoroutine == null)
            {
                skipTutorialCoroutine = TutorialManager.Instance.StartCoroutine(DelayedSkipTutorial());
            }
        }
    }

    private IEnumerator DelayedSkipTutorial()
    {
        Object.Destroy(tutorialDescriptionView.gameObject);
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.SkillAttackTutorial.ProgessSprites[2]);

        yield return new WaitForSeconds(1.5f);

        Object.Destroy(tutorialDescriptionView.gameObject);
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.SkillAttackTutorial.ProgessSprites[3]);

        yield return new WaitForSeconds(1.5f);

        TutorialManager.Instance.SkipTutorial();
    }
}