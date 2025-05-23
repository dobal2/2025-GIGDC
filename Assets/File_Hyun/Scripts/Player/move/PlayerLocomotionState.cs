using UnityEngine;

public class PlayerLocomotionState : PlayerState
{
    private bool shouldJump = false;

    public PlayerLocomotionState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override PlayerStateType StateType => PlayerStateType.Locomotion;

    public override void Enter()
    {
        PlayGroundedAnimation();
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

        UpdateAirAnimation();
    }

    void UpdateAirAnimation()
    {
        if (!player.isGrounded)
        {
            if (player.isJumping)
            {
                if (player.jumpTimeCounter >= player.MaxJumpTime * 0.6f)
                    player.Animator.Play("Player_Jump_End");
                else if (player.Rigidbody.linearVelocity.y > 0f)
                    player.Animator.Play("Player_Jump");
                else
                    player.Animator.Play("Player_Fall");
            }
            else
            {
                player.Animator.Play("Player_Fall");
            }
        }
        else
        {
            PlayGroundedAnimation();
        }
    }

    void PlayGroundedAnimation()
    {
        if (Mathf.Abs(player.MoveInput) > 0.01f)
            player.Animator.Play("Player_Walk");
        else
            player.Animator.Play("Player_Idle");
    }
}