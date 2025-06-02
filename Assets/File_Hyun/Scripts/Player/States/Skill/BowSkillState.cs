using UnityEngine;
using static PlayerController;

public class BowSkillState : PlayerState
{
    private readonly BowData bowData;

    public BowSkillState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
        bowData = player.AttackController.bowData;
    }

    public override PlayerStateType StateType => PlayerStateType.BowSkill;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation |
                                       RigidbodyConstraints2D.FreezePositionX |
                                       RigidbodyConstraints2D.FreezePositionY;
    }

    public override void Exit()
    {
        player.SetEffectState(PlayerEffectState.None);
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        player.AttackController.MarkSkillUsed();
        Debug.Log("[Skill] 스킬 사용됨 - 쿨타임 시작");
    }

    public override void Update()
    {
        if (!player.SkillHeld)
        {
            var arrowPrefab = bowData.skillArrowPrefab;
            if (arrowPrefab == null)
            {
                Debug.LogError("[Bow] 화살 프리팹이 할당되지 않았습니다.");
                return;
            }
            Object.Instantiate(arrowPrefab, (Vector2)player.transform.position, Quaternion.identity);
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {

    }
}