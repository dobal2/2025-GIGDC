using UnityEngine;

public class PlayerNormalState : PlayerState
{
    private bool shouldJump = false;

    public PlayerNormalState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "Normal";

    public override void Update()
    {
        player.AttackController.UpdateComboTimer();

        if (TryHandleSkillInput()) return;
        if (TryHandleComboContinue()) return;
        if (TryHandleComboStart()) return;
        if (TryHandleDash()) return;

        if (player.CrouchHeld && player.isGrounded)
        {
            stateMachine.ChangeState(new PlayerCrouchState(player, stateMachine));
            return;
        }

        if (!player.isJumping && player.jumpBufferTimer > 0f && (player.isGrounded || player.coyoteTimer > 0f))
        {
            shouldJump = true;
            player.jumpBufferTimer = 0f;
            player.AttackController.ResetCombo();
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