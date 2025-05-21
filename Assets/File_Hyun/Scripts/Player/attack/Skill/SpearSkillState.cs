using UnityEngine;

public class SpearSkillState : PlayerState
{
    public SpearSkillState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "SpearSkill";

    public override void Enter()
    {
        Debug.Log("아직 미구현된 상태입니다. [Idle 상태]로 전환합니다.");
        stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
    }
}