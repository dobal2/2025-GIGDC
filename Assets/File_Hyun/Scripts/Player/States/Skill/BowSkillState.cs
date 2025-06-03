using UnityEngine;
using static PlayerController;

public class BowSkillState : PlayerState
{
    private float skillStartTime;
    private bool fired;

    private readonly BowData bowData;

    public BowSkillState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
        bowData = player.AttackController.bowData;
    }

    public override PlayerStateType StateType => PlayerStateType.BowSkill;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        skillStartTime = Time.time;
        fired = false;
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation |
                                       RigidbodyConstraints2D.FreezePositionX |
                                       RigidbodyConstraints2D.FreezePositionY;
        player.SetEffectState(PlayerEffectState.BowSkillCharging);
    }

    public override void Exit()
    {
        player.SetEffectState(PlayerEffectState.None);
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        player.AttackController.MarkSkillUsed();
        Debug.Log("[Skill] 스킬 사용됨 - 쿨타임 시작");
    }

    public override void Update()
    {
        float chargeTime = Time.time - skillStartTime;

        if (chargeTime < 1.5f)
        {
            // 파랗게 변하기
        }

        if (!fired && (!player.SkillHeld || chargeTime >= 2f))
        {
            Fire(chargeTime);
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
        }
    }

    private void Fire(float chargeTime)
    {
        fired = true;
        player.SetEffectState(PlayerEffectState.BowSkillRelease);

        Vector2 fireOffset = new(bowData.fireOffset.x * player.facingDirection, bowData.fireOffset.y);
        Vector2 firePosition = (Vector2)player.transform.position + fireOffset;
        Vector2 fireDirection = new(player.facingDirection, 0);

        if (chargeTime < 1.5f)
        {
            float t = Mathf.Clamp01(chargeTime / 1.5f);
            float damage = Mathf.Lerp(bowData.minBowSkillDamage, bowData.maxBowSkillDamage, t);
            float speed = Mathf.Lerp(10f, 30f, t);
            float distance = Mathf.Lerp(10f, 30f, t);

            GameObject arrow = Object.Instantiate(bowData.skillArrowPrefab, firePosition, Quaternion.identity);
            if (arrow.TryGetComponent<SkillArrow>(out var arrowScript))
                arrowScript.Initialize(fireDirection, damage, speed, distance);
        }
        else
        {
            float damage = bowData.laserSkillDamage;
            float laserThickness = 4.5f;
            Vector2 boxSize = new(laserThickness, 1f);

            RaycastHit2D hit = Physics2D.BoxCast(
                firePosition,
                boxSize,
                0f,
                fireDirection,
                Mathf.Infinity,
                LayerMask.GetMask("Ground")
            );

            float baseDistance = hit.collider ? hit.distance : 50f;
            float hitDistance = baseDistance + (boxSize.x * 0.5f);

#if UNITY_EDITOR
            Vector2 boxCenter = firePosition + fireDirection.normalized * (hitDistance * 0.5f);
            float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
            DrawBoxCastBox(boxCenter, new Vector2(hitDistance, laserThickness), angle, Color.red, 0.5f);
#endif

            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                firePosition,
                boxSize,
                0f,
                fireDirection,
                hitDistance,
                LayerMask.GetMask("Enemy")
            );

            foreach (var h in hits)
            {
                if (h.collider.TryGetComponent<Monster>(out var monster))
                    monster.TakeDamage(damage);
            }
        }
    }

#if UNITY_EDITOR
    void DrawBoxCastBox(Vector2 center, Vector2 size, float angle, Color color, float duration)
    {
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        Vector2 half = size * 0.5f;

        Vector2 topLeft = center + (Vector2)(rot * new Vector2(-half.x, half.y));
        Vector2 topRight = center + (Vector2)(rot * new Vector2(half.x, half.y));
        Vector2 bottomRight = center + (Vector2)(rot * new Vector2(half.x, -half.y));
        Vector2 bottomLeft = center + (Vector2)(rot * new Vector2(-half.x, -half.y));

        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }
#endif
}