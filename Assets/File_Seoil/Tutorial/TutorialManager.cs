using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance {  get; private set; }
    public static TutorialVisualData VisualData => Instance.visualData;

    [SerializeField] private TutorialVisualData visualData;

    private List<ITutorialStep> tutorialSteps = new List<ITutorialStep>();
    private int tutorialStepIndex = 0;

    private void Awake()
    {
        Instance = this;

        tutorialSteps = new()
        {
            new MoveTutorialStep(),
            new DashTutorialStep(),
            new JumpTutorialStep(),
            new ChangeWeaponTutorialStep(),
            new AttackTutorialStep(),
            new ChainAttackTutorialStep(),
            new SkillAttackTutorialStep(),
            new CounterTutorialStep(),
            new CombatTutorialStep(),
        };
    }

    private void Start()
    {
        tutorialSteps[tutorialStepIndex].Enter();
    }

    private void Update()
    {
        tutorialSteps[tutorialStepIndex].Update();

        if (Input.GetKeyDown(KeyCode.Space))
            SkipTutorial();
    }

    public void SkipTutorial()
    {
        tutorialSteps[tutorialStepIndex].Exit();

        tutorialStepIndex++;
        if (tutorialStepIndex < tutorialSteps.Count)
            tutorialSteps[tutorialStepIndex].Enter();
        else
            OnEndTutorial();
    }

    private void OnEndTutorial()
    {
        Stage.Progress();
        SceneLoader.Instance.LoadScene(SceneType.Lobby_Over, SceneLoader.SceneChangeAnimation.Fade);
    }
}
