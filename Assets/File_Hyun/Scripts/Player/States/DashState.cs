using UnityEngine;
using static PlayerController;

public class DashState : PlayerState
{
    public DashState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override PlayerStateType StateType => PlayerStateType.Dash;

    public override void Enter()
    {
        player.SetEffectState(PlayerEffectState.Dash);
        player.Animator.Play("Dash");
        player.PlayClip(player.Dash);
        player.isNoClip = true;
        player.dashTimer = player.DashDuration;
        player.lastDashTime = Time.time;
        if (!player.isGrounded)
            player.canAirDash = false;

        player.jumpBufferTimer = 0f;
        player.isJumping = false;

        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        player.AttackController.CancelPush();
    }

    public override void Exit()
    {
        player.isNoClip = false;
        player.SetEffectState(PlayerEffectState.None);
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void FixedUpdate()
    {
        Vector2 dashDir = new(player.facingDirection, 0);
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, dashDir, 0.5f, player.DashStop);

        if (hit.collider)
        {
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
            return;
        }

        player.Rigidbody.linearVelocity = new Vector2(player.DashSpeed * player.facingDirection, 0);
        player.dashTimer -= Time.fixedDeltaTime;

        if (player.dashTimer <= 0f)
        {
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
        }
    }
}