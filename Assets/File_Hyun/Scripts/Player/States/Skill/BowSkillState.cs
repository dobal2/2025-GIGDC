using UnityEngine;
using static PlayerController;

public class BowSkillState : PlayerState
{
    private float skillStartTime;
    private bool fired;
    private bool skillStarted;
    private string prefix;
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
        if (!player.HasEnoughEnergy(player.AttackController.BowMinSkillEnergyCost))
        {
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
            return;
        }

        skillStarted = true;
        skillStartTime = Time.time;
        fired = false;
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation |
                                       RigidbodyConstraints2D.FreezePositionX |
                                       RigidbodyConstraints2D.FreezePositionY;
        prefix = player.isGrounded ? "Ground" : "Flying";
        player.Animator.Play($"Bow_{prefix}_Charging");
        player.PlayClip(player.BowSkillCharging);
        player.SetEffectState(PlayerEffectState.BowSkillCharging);
    }

    public override void Exit()
    {
        player.SetEffectState(PlayerEffectState.None);
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (skillStarted && fired)
            player.AttackController.MarkSkillUsed();
    }

    public override void Update()
    {
        float rawChargeTime = Time.time - skillStartTime;
        float fullChargeTime = player.AttackController.BowFullChargeTime;
        float autoFireTime = player.AttackController.BowAutoFireTime;
        float affordableChargeTime = player.AttackController.GetMaxAffordableBowChargeTime(player.CurrentEnergy);

        if (rawChargeTime > fullChargeTime && !fired)
            player.Animator.Play($"Bow_{prefix}_FullCharge");

        if (!fired && player.CurrentEnergy < player.AttackController.GetBowSkillEnergyCost(rawChargeTime))
        {
            Fire(affordableChargeTime);
            return;
        }

        if (!fired && (!player.SkillHeld || rawChargeTime >= autoFireTime))
            Fire(Mathf.Min(rawChargeTime, autoFireTime));

        if (fired)
        {
            AnimatorStateInfo animInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
            if (animInfo.IsName($"Bow_{prefix}_Shoot") && animInfo.normalizedTime >= 1f)
                stateMachine.ChangeState(new LocomotionState(player, stateMachine));
        }
    }

    private void Fire(float chargeTime)
    {
        int energyCost = player.AttackController.GetBowSkillEnergyCost(chargeTime);
        if (!player.TryConsumeEnergy(energyCost))
        {
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
            return;
        }

        fired = true;
        player.Animator.Play($"Bow_{prefix}_Shoot");
        player.PlayClip(player.BowSkillRelease);

        if (chargeTime > player.AttackController.BowFullChargeTime)
            player.SetEffectState(PlayerEffectState.BowSkillFullChargeRelease);
        else
            player.SetEffectState(PlayerEffectState.BowSkillRelease);

        CameraUtility.ShakeCamera(
            duration: 0.3f,
            strength: Mathf.Lerp(0.05f, 0.5f, Mathf.Clamp01(chargeTime / player.AttackController.BowFullChargeTime)),
            vibrato: 10,
            randomness: 90,
            fadeOut: true
        );

        Vector2 fireOffset = new(bowData.fireOffset.x * player.facingDirection, bowData.fireOffset.y);
        Vector2 firePosition = (Vector2)player.transform.position + fireOffset;
        Vector2 fireDirection = new(player.facingDirection, 0);

        if (chargeTime < player.AttackController.BowFullChargeTime)
        {
            float t = Mathf.Clamp01(chargeTime / player.AttackController.BowFullChargeTime);
            float damage = Mathf.Lerp(bowData.minBowSkillDamage, bowData.maxBowSkillDamage, t);
            float speed = Mathf.Lerp(10f, 30f, t);
            float distance = Mathf.Lerp(10f, 30f, t);

            GameObject arrow = Object.Instantiate(bowData.skillArrowPrefab, firePosition, Quaternion.identity);
            if (arrow.TryGetComponent<SkillArrow>(out SkillArrow arrowScript))
                arrowScript.Initialize(fireDirection, damage, speed, distance);
            return;
        }

        float laserThickness = bowData.laserThickness;
        Vector2 boxSize = new(laserThickness, 1f);

        RaycastHit2D hit = Physics2D.BoxCast(
            firePosition,
            boxSize,
            0f,
            fireDirection,
            Mathf.Infinity,
            LayerMask.GetMask("Ground", "Wall")
        );

        float baseDistance = hit.collider ? hit.distance : 50f;
        float hitDistance = baseDistance + (boxSize.x * 0.5f);

        Vector2 boxCenter = firePosition + fireDirection.normalized * (hitDistance * 0.5f);
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        DrawBoxCastBox(boxCenter, new Vector2(hitDistance, laserThickness), angle, Color.red, 0.5f);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            firePosition,
            boxSize,
            0f,
            fireDirection,
            hitDistance,
            LayerMask.GetMask("Enemy")
        );

        foreach (RaycastHit2D h in hits)
        {
            if (h.collider.TryGetComponent<Monster>(out Monster monster))
            {
                monster.TakeDamage(bowData.laserSkillDamage);
                monster.KnockBack(
                    attacker: player.transform,
                    knockBackForce: 0.5f * bowData.laserSkillDamage,
                    knockBackAngle: 0,
                    duration: 0.3f
                );
            }
        }
    }

    private void DrawBoxCastBox(Vector2 center, Vector2 size, float angle, Color color, float duration)
    {
#if UNITY_EDITOR
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
#endif
    }
}