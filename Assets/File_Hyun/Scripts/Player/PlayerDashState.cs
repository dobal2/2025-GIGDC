using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "Dash";
    public override void Enter()
    {
        player.dashTimer = player.DashDuration;
        player.lastDashTime = Time.time;
        if (!player.isGrounded)
            player.canAirDash = false;

        player.jumpBufferTimer = 0f;
        player.dashBufferTimer = 0f;
        player.isJumping = false;

        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public override void FixedUpdate()
    {
        Vector2 dashDir = new(player.facingDirection, 0);
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, dashDir, 0.5f, player.DashStop);

        if (hit.collider)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
            return;
        }

        player.Rigidbody.linearVelocity = new Vector2(player.DashSpeed * player.facingDirection, 0);
        player.dashTimer -= Time.fixedDeltaTime;

        if (player.dashTimer <= 0f)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    public override void Exit()
    {
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}