using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private bool shouldJump = false;

    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "Idle";

    public override void Enter()
    {
        player.Animator.Play("Player_Idle");
    }

    public override void Update()
    {
        player.AttackController.UpdateComboTimer();

        if (TryHandleSkillInput()) return;
        if (TryHandleComboContinue()) return;
        if (TryHandleComboStart()) return;
        if (TryHandleDash()) return;

        if (!player.isJumping && player.jumpBufferTimer > 0f && (player.isGrounded || player.coyoteTimer > 0f))
        {
            shouldJump = true;
            player.jumpBufferTimer = 0f;
            player.AttackController.ResetCombo();
        }

        if (Mathf.Abs(player.MoveInput) > 0.01f)
        {
            stateMachine.ChangeState(new PlayerMoveState(player, stateMachine));
        }
    }

    void UpdateAirAnimation()
    {
        if (!player.isGrounded)
        {
            if (player.isJumping && player.Rigidbody.linearVelocity.y > 0f)
            {
                    //player.Animator.Play("Player_Jump");
            }
            else
            {
                    player.Animator.Play("Player_Fall");
            }
        }
        else
        {
            if (Mathf.Abs(player.MoveInput) > 0.01f)
            {
                    player.Animator.Play("Player_Walk");
            }
            else
            {
                    player.Animator.Play("Player_Idle");
            }
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

        player.HandleJump();
        player.HandleFastFall();
        player.Rigidbody.linearVelocity = new Vector2(0, player.Rigidbody.linearVelocity.y);

        UpdateAirAnimation();
    }
}