using UnityEngine;
using System.Collections;

public class SpearAttackState : PlayerState
{
    private readonly SpearData spearData;
    private int lastComboStep = 0;

    public SpearAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { spearData = player.AttackController.spearData; }

    public override PlayerStateType StateType => PlayerStateType.SpearAttack;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;

        if (!player.isGrounded)
            player.Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;

        lastComboStep = player.AttackController.ComboStep;

        if (lastComboStep >= 1 && lastComboStep <= spearData.MaxCombo)
        {
            float delay = spearData.spearComboInfos[lastComboStep - 1].damageDelay;
            player.StartCoroutine(DelayedHit(lastComboStep, delay));
        }
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
            player.AttackController.ContinueCombo();

            int currentStep = player.AttackController.ComboStep;
            if (currentStep != lastComboStep)
            {
                lastComboStep = currentStep;
                if (currentStep >= 1 && currentStep <= spearData.MaxCombo)
                {
                    float delay = spearData.spearComboInfos[currentStep - 1].damageDelay;
                    player.StartCoroutine(DelayedHit(currentStep, delay));
                }
            }

            return;
        }

        if (player.AttackController.CanMove)
        {
            if (player.AttackController.IsFinalComboStep)
                player.NotifyChainAttackFinished();

            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
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

    private IEnumerator DelayedHit(int step, float delay)
    {
        yield return new WaitForSeconds(delay);
        TryHitComboEnemy(step);
    }

    private void TryHitComboEnemy(int step)
    {
        if (step < 1 || step > spearData.MaxCombo)
            return;

        SpearComboInfo info = spearData.spearComboInfos[step - 1];
        Vector2 offset = info.localOffset;
        offset.x *= player.facingDirection;
        Vector2 hitCenter = (Vector2)player.transform.position + offset;
        float radius = info.range;

        bool hasHitMonster = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitCenter, radius, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Monster>(out var monster))
            {
                monster.TakeDamage(info.damage);
                monster.KnockBack(
                    attacker: player.transform,
                    knockBackForce: 10 * info.pushDistance,
                    knockBackAngle: 0,
                    duration: 1.2f * info.pushTime
                );
                hasHitMonster = true;
            }
        }

        if (hasHitMonster)
        {
            CameraUtility.ShakeCamera(
                duration: 0.3f,
                strength: 0.2f,
                vibrato: 10,
                randomness: 90,
                fadeOut: true
            );
        }

        DebugDrawCrossX(hitCenter, radius, 0.2f);
    }

    private void DebugDrawCrossX(Vector2 center, float radius, float duration)
    {
#if UNITY_EDITOR
        Color color = Color.red;
        Debug.DrawLine(center + new Vector2(-radius, 0), center + new Vector2(radius, 0), color, duration);
        Debug.DrawLine(center + new Vector2(0, -radius), center + new Vector2(0, radius), color, duration);
#endif
    }
}