using UnityEngine;

public class ChainAttackTutorialStep : TutorialStepBase
{
    private TutorialDescriptionView tutorialDescriptionView;

    public override void Enter()
    {
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.ChainAttackTutorial.ProgessSprites[0]);

        PlayerController.Instance.OnChainAttackFinished += OnPlayerChainAttack;
    }

    public override void Exit()
    {
        Object.Destroy(tutorialDescriptionView.gameObject);
        PlayerController.Instance.OnChainAttackFinished -= OnPlayerChainAttack;
    }

    public override void Update()
    {
        
    }

    private void OnPlayerChainAttack()
    {
        TutorialManager.Instance.SkipTutorial();
    }
}