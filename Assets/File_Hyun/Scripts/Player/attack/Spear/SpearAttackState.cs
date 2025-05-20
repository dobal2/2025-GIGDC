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

        if (!player.isGrounded)
            player.Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;
    }

    public override void Exit()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.Rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    public override void Update()
    {
        player.AttackController.UpdateComboTimer();

        if (player.dashBufferTimer > 0f && Time.time >= player.lastDashTime + player.DashCooldown)
        {
            if (player.isGrounded || player.canAirDash)
            {
                stateMachine.ChangeState(new PlayerDashState(player, stateMachine));
                return;
            }
        }

        if (player.skillRequested)
        {
            player.ConsumeSkillRequest();
            stateMachine.ChangeState(player.AttackController.GetSkillState(stateMachine));
            return;
        }

        if (player.AttackBuffered && player.AttackController.CanComboInput)
        {
            player.ConsumeAttackBuffer();
            player.AttackController.MarkComboInputReceived();
            player.AttackController.ContinueCombo();
            return;
        }

        if (player.AttackController.CanMove)
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