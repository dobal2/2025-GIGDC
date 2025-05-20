using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "Idle";

    public override void Update()
    {
        if (player.skillRequested)
        {
            player.ConsumeSkillRequest();
            stateMachine.ChangeState(player.AttackController.GetSkillState(stateMachine));
            return;
        }

        // 콤보 연계 (comboKeep 중)
        if (player.AttackBuffered && player.AttackController.CanComboInput)
        {
            player.ConsumeAttackBuffer();
            player.AttackController.MarkComboInputReceived();
            player.AttackController.ContinueCombo();
            stateMachine.ChangeState(new SpearAttackState(player, stateMachine));
            return;
        }

        // 콤보 시작 (지상 or 공중 1회 허용)
        if (player.AttackBuffered && (player.isGrounded || player.AttackController.CanStartAirborneCombo))
        {
            player.ConsumeAttackBuffer();
            stateMachine.ChangeState(player.AttackController.GetAttackState(stateMachine));
            return;
        }

        if (player.CrouchHeld && player.isGrounded)
        {
            stateMachine.ChangeState(new PlayerCrouchState(player, stateMachine));
            return;
        }

        if (player.dashBufferTimer > 0f && Time.time >= player.lastDashTime + player.DashCooldown)
        {
            if (player.isGrounded || player.canAirDash)
            {
                stateMachine.ChangeState(new PlayerDashState(player, stateMachine));
                return;
            }
        }

        if (player.jumpBufferTimer > 0 && (player.isGrounded || player.coyoteTimer > 0f))
        {
            player.coyoteTimer = 0f;
            player.isJumping = true;
            player.isGrounded = false;
            player.jumpTimeCounter = 0f;
            player.jumpBufferTimer = 0f;
        }

        if (Mathf.Abs(player.MoveInput) > 0.01f)
        {
            stateMachine.ChangeState(new PlayerNormalState(player, stateMachine));
        }
    }


    public override void FixedUpdate()
    {
        player.HandleJump();
        player.HandleFastFall();
        player.Rigidbody.linearVelocity = new Vector2(0, player.Rigidbody.linearVelocity.y);
    }
}