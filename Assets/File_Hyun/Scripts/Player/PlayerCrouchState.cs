using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    private Vector2 crouchSize;
    private Vector2 crouchOffset;

    public PlayerCrouchState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
        var size = player.OriginalColliderSize;
        crouchSize = new Vector2(size.x, size.y * player.CrouchColliderHeightMultiplier);

        var offset = player.OriginalColliderOffset;
        crouchOffset = new Vector2(offset.x, offset.y + (size.y * (player.CrouchColliderHeightMultiplier - 1f) / 2f));
    }

    public override string Name => "Crouch";
    public override void Enter()
    {
        player.BoxCollider.size = crouchSize;
        player.BoxCollider.offset = crouchOffset;
    }

    public override void Update()
    {
        if (!player.CrouchHeld || !player.isGrounded)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        player.HandleMove(player.MoveSpeed * player.CrouchSpeedMultiplier);
    }

    public override void Exit()
    {
        player.BoxCollider.size = player.OriginalColliderSize;
        player.BoxCollider.offset = player.OriginalColliderOffset;
    }
}