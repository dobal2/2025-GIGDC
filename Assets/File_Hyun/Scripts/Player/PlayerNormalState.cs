using UnityEngine;

public class PlayerNormalState : PlayerState
{
    public PlayerNormalState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "Normal";

    public override void Update()
    {
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
            player.isJumping = true;
            player.jumpTimeCounter = 0f;
            player.jumpBufferTimer = 0f;
            player.coyoteTimer = 0f;
        }

        if (Mathf.Abs(player.MoveInput) <= 0.01f)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        player.HandleMove(player.MoveSpeed);
        player.HandleJump();
        player.HandleFastFall();
    }
}