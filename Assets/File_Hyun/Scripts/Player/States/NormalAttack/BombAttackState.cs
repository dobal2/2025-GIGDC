using UnityEngine;

public class BombAttackState : PlayerState
{
    private bool bombThrown;
    private bool hasAutoTarget;
    private Vector2 autoTargetPoint;

    private readonly BombData bombData;

    public BombAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
        bombData = player.AttackController.bombData;
    }

    public override PlayerStateType StateType => PlayerStateType.BombAttack;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        if (!player.isGrounded)
            player.Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;

        hasAutoTarget = false;
        if (bombData.normalBombAutoTargetRange > 0f)
            hasAutoTarget = CombatTargetingUtility.TryFindNearestEnemyPoint(
                player.transform.position,
                bombData.normalBombAutoTargetRange,
                LayerMask.GetMask("Enemy"),
                out autoTargetPoint
            );

        if (hasAutoTarget)
            FaceTarget(autoTargetPoint.x);

        player.AttackController.StartCombo();
        bombThrown = false;
    }

    public override void Exit()
    {
        player.Rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    public override void Update()
    {
        player.AttackController.UpdateComboTimer();

        if (TryHandleDash())
            return;
        if (TryHandleSkillInput())
            return;

        AnimatorStateInfo animInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
        if (animInfo.IsName("End") || animInfo.IsName("Flying_End"))
        {
            ThrowBomb();
            if (animInfo.normalizedTime >= 1f)
            {
                if (player.AttackController.IsFinalComboStep)
                    player.NotifyChainAttackFinished();

                stateMachine.ChangeState(new LocomotionState(player, stateMachine));
            }
        }
    }

    private void ThrowBomb()
    {
        if (bombData.normalBombPrefab == null)
        {
            Debug.LogError("[Bomb] 폭탄 프리팹이 비어있습니다.");
            return;
        }

        if (bombThrown)
            return;

        bombThrown = true;

        Vector2 offset = bombData.localOffset;
        offset.x *= player.facingDirection;
        Vector2 spawnPos = (Vector2)player.transform.position + offset;

        GameObject bomb = Object.Instantiate(bombData.normalBombPrefab, spawnPos, Quaternion.identity);
        Vector2 direction = new(player.facingDirection, 0f);
        NormalBomb normalBomb = bomb.GetComponent<NormalBomb>();

        if (hasAutoTarget)
        {
            normalBomb.Initialize(
                direction,
                bombData.damage,
                bombData.throwAngle,
                bombData.throwSpeed,
                bombData.bombExplosionRadius,
                autoTargetPoint,
                bombData.normalBombAutoTargetArcHeight
            );
        }
        else
        {
            normalBomb.Initialize(
                direction,
                bombData.damage,
                bombData.throwAngle,
                bombData.throwSpeed,
                bombData.bombExplosionRadius
            );
        }

        player.AttackController.MarkBombThrown();
    }

    private void FaceTarget(float targetX)
    {
        float deltaX = targetX - player.transform.position.x;
        if (Mathf.Abs(deltaX) <= 0.01f)
            return;

        player.facingDirection = deltaX > 0f ? 1 : -1;
        player.transform.rotation = Quaternion.Euler(0f, player.facingDirection == -1 ? 180f : 0f, 0f);
    }
}