using UnityEngine;

public class ChangeWeaponTutorialStep : TutorialStepBase
{
    private TutorialDescriptionView tutorialDescriptionView;
    private int index = 0;

    public override void Enter()
    {
        tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
        tutorialDescriptionView.Initialize(TutorialManager.VisualData.ChangeWeaponTutorial.ProgessSprites[index]);

        PlayerController.Instance.OnChangeWeapon += OnPlayerChangeWeapon;
    }

    public override void Exit()
    {
        Object.Destroy(tutorialDescriptionView.gameObject);
        PlayerController.Instance.OnChangeWeapon -= OnPlayerChangeWeapon;
    }

    public override void Update()
    {
        
    }

    private void OnPlayerChangeWeapon(WeaponType weaponType)
    {
        index++;
        if(index <= 1)
        {
            Object.Destroy(tutorialDescriptionView.gameObject);
            tutorialDescriptionView = UIManager.Instance.Show<TutorialDescriptionView>();
            tutorialDescriptionView.Initialize(TutorialManager.VisualData.ChangeWeaponTutorial.ProgessSprites[index]);
        }
        else if(weaponType == WeaponType.Spear)
        {
            TutorialManager.Instance.SkipTutorial();
        }
    }
}