using UnityEngine;
using static PlayerController;

public class LocomotionState : PlayerState
{
    private bool shouldJump = false;
    private bool pendingEndJump = false;

    public LocomotionState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override PlayerStateType StateType => PlayerStateType.Locomotion;

    public override void Enter()
    {
        player.Rigidbody.WakeUp();
    }

    public override void Exit()
    {
        player.SetEffectState(PlayerEffectState.None);
        player.isJumping = false;
    }

    public override void Update()
    {
        player.AttackController.UpdateComboTimer();

        if (TryHandleSkillInput()) return;
        if (TryHandleComboContinue()) return;
        if (TryHandleComboStart()) return;
        if (TryHandleDash()) return;

        if (player.ChangePressed && !player.isJumping) player.AttackController.TrySwitchWeapon();

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

            player.Animator.Play("Jump");
            player.PlayClip(player.Jump);
        }

        if (player.isJumping && (!player.JumpHeld || player.jumpTimeCounter >= player.MaxJumpTime || player.isTouchingCeiling))
        {
            player.isJumping = false;
            pendingEndJump = true;
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
            if (pendingEndJump)
            {
                player.Animator.Play("Endjump");
                pendingEndJump = false;
            }
            else if (!player.isJumping)
            {
                if (!player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Endjump"))
                    player.Animator.Play("Fall");
            }
        }
        else if (player.isGrounded)
        {
            PlayGroundedAnimation();
        }
    }

    void PlayGroundedAnimation()
    {
        if (Mathf.Abs(player.MoveInput) > 0.01f)
        {
            player.Animator.Play("Walk");
            player.SetEffectState(PlayerEffectState.GroundWalkDust);
            player.Walk.Play();
        }
        else
        {
            player.Animator.Play("Idle");
            player.SetEffectState(PlayerEffectState.None);
            player.Walk.Stop();
        }
    }
}