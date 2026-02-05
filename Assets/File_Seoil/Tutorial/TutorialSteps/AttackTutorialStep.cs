using UnityEngine;

public class AttackTutorialStep : TutorialStepBase
{
    private TutorialDescriptionView tutorialDescriptionView;

    public override void Enter()
    {
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.AttackTutorial.ProgessSprites[0]);
    }

    public override void Exit()
    {
        Object.Destroy(tutorialDescriptionView.gameObject);
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TutorialManager.Instance.SkipTutorial();
        }
    }
}