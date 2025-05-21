using UnityEngine;

public class GenericAttackState : PlayerState
{
    public GenericAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "GenericAttack";
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;

        if (!player.isGrounded)
            player.Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;

        player.AttackController.OnAttackEnter(); // 무기별 특화 진입 동작 (예: 투사체 발사 등)
    }

    public override void Exit()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.Rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;

        player.AttackController.OnAttackExit(); // 무기별 특화 종료 동작 (예: 효과 정리 등)
    }

    public override void Update()
    {
        player.AttackController.UpdateComboTimer();

        // 공격 중 대시 캔슬
        if (player.dashBufferTimer > 0f && Time.time >= player.lastDashTime + player.DashCooldown)
        {
            if (player.isGrounded || player.canAirDash)
            {
                stateMachine.ChangeState(new PlayerDashState(player, stateMachine));
                return;
            }
        }

        // 스킬 전이
        if (player.skillRequested)
        {
            player.ConsumeSkillRequest();
            stateMachine.ChangeState(player.AttackController.GetSkillState(stateMachine));
            return;
        }

        // 콤보 연계
        if (player.AttackBuffered && player.AttackController.CanComboInput)
        {
            player.ConsumeAttackBuffer();
            player.AttackController.MarkComboInputReceived();
            player.AttackController.ContinueCombo();
            return;
        }

        // 공격 종료
        if (player.AttackController.CanMove)
        {
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