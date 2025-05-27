using UnityEngine;

public class SpearAttackState : PlayerState
{
    public SpearAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override PlayerStateType StateType => PlayerStateType.SpearAttack;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;

        if (!player.isGrounded)
            player.Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;
    }

    public override void Exit()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.Rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    public override void Update()
    {
        player.AttackController.UpdateComboTimer();

        if (TryHandleDash()) return;
        if (TryHandleSkillInput()) return;

        if (player.AttackBuffered && player.AttackController.CanComboInput)
        {
            player.ConsumeAttackBuffer();
            player.AttackController.MarkComboInputReceived();
            player.AttackController.ContinueCombo();
            return;
        }

        if (player.AttackController.CanMove)
        {
            stateMachine.ChangeState(new PlayerLocomotionState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        if (player.AttackController.IsPushing)
        {
            float push = player.AttackController.GetPushDelta(Time.fixedDeltaTime);
            player.Rigidbody.MovePosition(player.Rigidbody.position + new Vector2(push * player.facingDirection, 0f));
        }
    }
}