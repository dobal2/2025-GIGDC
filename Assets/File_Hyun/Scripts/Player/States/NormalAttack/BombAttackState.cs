using UnityEngine;

public class BombAttackState : PlayerState
{
    private float timer;
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

        player.AttackController.StartCombo(); // 콤보 1단계 강제 시작 → 애니메이션 재생됨

        timer = 0f;
        bombThrown = false;
    }

    public override void Exit()
    {
        player.Rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        player.AttackController.UpdateComboTimer();

        if (!bombThrown && timer >= bombData.throwDelay)
        {
            ThrowBomb();
            bombThrown = true;
        }

        if (TryHandleDash()) return;
        if (TryHandleSkillInput()) return;

        AnimatorStateInfo animInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
        if (animInfo.IsName("1") || animInfo.IsName("Flying_1"))
        {
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
    }
}