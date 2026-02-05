using UnityEngine;

public class MoveTutorialStep : TutorialStepBase
{
    private TutorialDescriptionView tutorialDescriptionView;

    public override void Enter()
    {
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.MoveTutorial.ProgessSprites[0]);
    }

    public override void Exit()
    {
        Object.Destroy(tutorialDescriptionView.gameObject);
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            TutorialManager.Instance.SkipTutorial();
        }
    }
}