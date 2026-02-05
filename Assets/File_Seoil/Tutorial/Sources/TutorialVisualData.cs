using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TutorialVisualData", menuName = "Scriptable Objects/TutorialVisualData")]
public class TutorialVisualData : ScriptableObject
{
    [SerializeField] private TutorialVisualSet moveTutorial;
    [SerializeField] private TutorialVisualSet dashTutorial;
    [SerializeField] private TutorialVisualSet jumpTutorial;
    [SerializeField] private TutorialVisualSet changeWeaponTutorial;
    [SerializeField] private TutorialVisualSet attackTutorial;
    [SerializeField] private TutorialVisualSet chainAttackTutorial;
    [SerializeField] private TutorialVisualSet skllAttackTutorial;
    [SerializeField] private TutorialVisualSet counterTutorial;
    [SerializeField] private TutorialVisualSet combatTutorial;

    public TutorialVisualSet MoveTutorial => moveTutorial;
    public TutorialVisualSet DashTutorial => dashTutorial;
    public TutorialVisualSet JumpTutorial => jumpTutorial;
    public TutorialVisualSet ChangeWeaponTutorial => changeWeaponTutorial;
    public TutorialVisualSet AttackTutorial => attackTutorial;
    public TutorialVisualSet ChainAttackTutorial => chainAttackTutorial;
    public TutorialVisualSet SkillAttackTutorial => skllAttackTutorial;
    public TutorialVisualSet CounterTutorial => counterTutorial;
    public TutorialVisualSet CombatTutorial => combatTutorial;

    [Serializable]
    public class TutorialVisualSet
    {
        public List<Sprite> ProgessSprites;
        public Sprite SuccesSprite;
        public Sprite FailSprite;
    }
}
