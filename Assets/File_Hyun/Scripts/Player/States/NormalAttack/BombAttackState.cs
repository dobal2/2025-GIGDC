using UnityEngine;

public class BombAttackState : PlayerState
{
    private bool bombThrown;

    private readonly BombData bombData;

    public BombAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { bombData = player.AttackController.bombData; }

    public override PlayerStateType StateType => PlayerStateType.BombAttack;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        if (!player.isGrounded)
            player.Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;

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

        if (TryHandleDash()) return;
        if (TryHandleSkillInput()) return;

        AnimatorStateInfo animInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
        if (animInfo.IsName("End") || animInfo.IsName("Flying_End"))
        {
            ThrowBomb();
            if (animInfo.normalizedTime >= 1f)
                stateMachine.ChangeState(new LocomotionState(player, stateMachine));
        }
    }

    private void ThrowBomb()
    {
        if (bombData.normalBombPrefab == null)
        {
            Debug.LogError("[Bomb] 폭탄 프리팹이 비어있습니다.");
            return;
        }

        if (bombThrown) return;

        bombThrown = true;
        Vector2 offset = bombData.localOffset;
        offset.x *= player.facingDirection;
        Vector2 spawnPos = (Vector2)player.transform.position + offset;

        GameObject bomb = Object.Instantiate(bombData.normalBombPrefab, spawnPos, Quaternion.identity);
        Vector2 direction = new(player.facingDirection, 0);

        bomb.GetComponent<NormalBomb>().Initialize(
            direction,
            bombData.damage,
            bombData.throwAngle,
            bombData.throwSpeed,
            bombData.bombExplosionRadius
        );
        player.AttackController.MarkBombThrown();
    }
}