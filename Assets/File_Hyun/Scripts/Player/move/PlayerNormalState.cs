using UnityEngine;

public class PlayerNormalState : PlayerState
{
    private bool shouldJump = false;

    public PlayerNormalState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "Normal";

    public override void Update()
    {
        if (player.skillRequested)
        {
            player.ConsumeSkillRequest();
            stateMachine.ChangeState(player.AttackController.GetSkillState(stateMachine));
            return;
        }

        if (player.AttackBuffered)
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

        if (!player.isJumping && player.jumpBufferTimer > 0 && (player.isGrounded || player.coyoteTimer > 0f))
        {
            shouldJump = true;
            player.jumpBufferTimer = 0f;
        }

        if (Mathf.Abs(player.MoveInput) <= 0.01f)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        if (shouldJump)
        {
            player.coyoteTimer = 0f;
            player.isJumping = true;
            player.isGrounded = false;
            player.jumpTimeCounter = 0f;
            shouldJump = false;
        }

        player.HandleMove(player.MoveSpeed);
        player.HandleJump();
        player.HandleFastFall();
    }
}