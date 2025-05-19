using UnityEngine;

public class SpearAttackState : PlayerState
{
    public SpearAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "SpearAttack";
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.AttackController.StartCombo();
    }

    public override void Exit()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        player.AttackController.UpdateComboTimer();

        if (player.skillRequested)
        {
            player.ConsumeSkillRequest();
            stateMachine.ChangeState(player.AttackController.GetSkillState(stateMachine));
            return;
        }

        // 콤보 입력 시 계속 공격 상태 유지
        if (player.AttackBuffered && player.AttackController.CanComboInput)
        {
            player.ConsumeAttackBuffer();
            player.AttackController.MarkComboInputReceived();
            player.AttackController.ContinueCombo();
            return;
        }

        // push가 끝났으면 이동 가능한 상태로 전이
        if (player.AttackController.CanComboInput)
        {
            if (Mathf.Abs(player.MoveInput) > 0.01f)
                stateMachine.ChangeState(new PlayerNormalState(player, stateMachine));
            else
                stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        if (player.AttackController.IsPushing)
        {
            float push = player.AttackController.GetPushDelta(Time.fixedDeltaTime);
            player.Rigidbody.MovePosition(player.Rigidbody.position + new Vector2(push * player.facingDirection, 0f));
        }
    }
}