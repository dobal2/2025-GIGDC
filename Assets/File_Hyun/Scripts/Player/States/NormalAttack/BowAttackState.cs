using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BowAttackState : PlayerState
{
    private BowData bowData;

    public BowAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
        bowData = player.AttackController.bowData;
    }

    public override PlayerStateType StateType => PlayerStateType.BowAttack;
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

    private void FireArrow(Vector2 localOffset)
    {
        var arrowPrefab = bowData.normalArrowPrefab;
        if (arrowPrefab == null)
        {
            Debug.LogWarning("[Bow] 화살 프리팹이 할당되지 않았습니다.");
            return;
        }

        Vector2 firePos = (Vector2)player.transform.position + new Vector2(localOffset.x * player.facingDirection, localOffset.y);
        GameObject arrow = Object.Instantiate(arrowPrefab, firePos, Quaternion.identity);

        //Debug.Log($"[Bow] 화살 발사됨 at {firePos}");
    }
}