using UnityEngine;

public class BombSkillState : PlayerState
{
    private readonly BombData bombData;
    private bool bombsThrown;
    private bool skillStarted;
    private string endAnimName;

    public BombSkillState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
        bombData = player.AttackController.bombData;
    }

    public override PlayerStateType StateType => PlayerStateType.BombSkill;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        if (!player.TryConsumeEnergy(player.AttackController.BombSkillEnergyCost))
        {
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
            return;
        }

        skillStarted = true;
        bombsThrown = false;
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation |
                                       RigidbodyConstraints2D.FreezePositionX |
                                       RigidbodyConstraints2D.FreezePositionY;

        string prefix = player.isGrounded ? "Ground" : "Flying";
        endAnimName = $"Bomb_{prefix}_End";
        player.Animator.Play($"Bomb_{prefix}_Throwing");
    }

    public override void Exit()
    {
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (skillStarted && bombsThrown)
            player.AttackController.MarkSkillUsed();
    }

    public override void Update()
    {
        AnimatorStateInfo animInfo = player.Animator.GetCurrentAnimatorStateInfo(0);

        if (!bombsThrown && animInfo.IsName(endAnimName) && animInfo.normalizedTime < 0.1f)
            ThrowBombs();

        if (bombsThrown && animInfo.IsName(endAnimName) && animInfo.normalizedTime >= 1f)
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
    }

    private void ThrowBombs()
    {
        bombsThrown = true;
        int count = Random.Range(bombData.bombSkillMinNumber, bombData.bombSkillMaxNumber + 1);
        Vector2 baseOffset = bombData.localOffset;
        baseOffset.x *= player.facingDirection;
        Vector2 origin = (Vector2)player.transform.position + baseOffset;

        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(bombData.bombSkillMinThrowAngle, bombData.bombSkillMaxThrowAngle);
            float fuse = Random.Range(bombData.bombSkillMinExplosionTime, bombData.bombSkillMaxExplosionTime);
            float speed = Random.Range(bombData.bombSkillMinThrowSpeed, bombData.bombSkillMaxThrowSpeed);

            GameObject bomb = Object.Instantiate(bombData.skillBombPrefab, origin, Quaternion.identity);
            if (bomb.TryGetComponent<SkillBomb>(out SkillBomb skillBomb))
            {
                skillBomb.Initialize(
                    new Vector2(player.facingDirection, 0),
                    bombData.bombSkillDamage,
                    angle,
                    speed,
                    bombData.bombExplosionRadius,
                    fuse
                );
            }
        }
    }
}