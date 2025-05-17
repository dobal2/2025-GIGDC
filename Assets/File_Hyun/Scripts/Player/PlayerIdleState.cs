using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "Idle";

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