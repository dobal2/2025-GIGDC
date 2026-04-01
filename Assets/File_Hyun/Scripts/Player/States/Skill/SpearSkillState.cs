using UnityEngine;
using static PlayerController;

public class SpearSkillState : PlayerState
{
    private enum SkillPhase
    {
        Charging,
        Moving,
        WaitingForLanding,
        Landing
    }

    private enum SkillMode
    {
        Ground,
        LowAir,
        HighAir
    }

    private SkillMode mode;
    private SkillPhase phase;

    private float dashSpeed;

    private bool landingTriggered;
    private bool forceGroundedIgnore;
    private bool skillStarted;
    private bool useAutoTargetTrajectory;
    private bool hasAutoTargetLandingPoint;
    private Vector2 autoTargetLandingPoint;

    private readonly SpearData spearData;

    public SpearSkillState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
        spearData = player.AttackController.spearData;
    }

    public override PlayerStateType StateType => PlayerStateType.SpearSkill;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        if (!player.TryConsumeEnergy(player.AttackController.SpearSkillEnergyCost))
        {
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
            return;
        }

        skillStarted = true;
        landingTriggered = false;
        forceGroundedIgnore = true;
        useAutoTargetTrajectory = false;

        player.Rigidbody.linearVelocity = Vector2.zero;
        player.isNoClip = true;

        hasAutoTargetLandingPoint = TryResolveAutoTargetLandingPoint(out autoTargetLandingPoint);
        if (hasAutoTargetLandingPoint)
            FaceTarget(autoTargetLandingPoint.x);

        if (player.isGrounded)
        {
            player.Animator.Play("Spear_Ground_Jump");
            mode = SkillMode.Ground;
            phase = SkillPhase.Moving;
            dashSpeed = 14f;

            useAutoTargetTrajectory = hasAutoTargetLandingPoint && TryLaunchToTarget(autoTargetLandingPoint);
            if (!useAutoTargetTrajectory)
            {
                Vector2 velocity = new(player.facingDirection * dashSpeed, spearData.jumpSpeed);
                player.Rigidbody.linearVelocity = velocity;
            }
        }
        else if (player.isLowAir)
        {
            player.Animator.Play("Spear_GroundAir_Jump");
            mode = SkillMode.LowAir;
            phase = SkillPhase.Moving;
            dashSpeed = 18f;

            useAutoTargetTrajectory = hasAutoTargetLandingPoint && TryLaunchToTarget(autoTargetLandingPoint);
            if (!useAutoTargetTrajectory)
            {
                Vector2 velocity = new(player.facingDirection * dashSpeed, spearData.jumpSpeed * 0.5f);
                player.Rigidbody.linearVelocity = velocity;
            }
        }
        else
        {
            player.Animator.Play("Spear_Flying_Charge");
            mode = SkillMode.HighAir;
            phase = SkillPhase.Charging;
            player.Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX |
                                           RigidbodyConstraints2D.FreezePositionY;
        }

        player.StartCoroutine(ResetForceGroundedIgnore());
    }

    public override void Exit()
    {
        player.isNoClip = false;
        player.SetEffectState(PlayerEffectState.None);
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        player.Rigidbody.linearVelocity = Vector2.zero;

        if (skillStarted)
            player.AttackController.MarkSkillUsed();
    }

    public override void Update()
    {
        if (phase == SkillPhase.Charging)
        {
            AnimatorStateInfo animInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
            if (animInfo.IsName("Spear_Flying_Charge") && animInfo.normalizedTime >= 1f)
            {
                if (hasAutoTargetLandingPoint)
                {
                    player.transform.position = new Vector3(
                        autoTargetLandingPoint.x,
                        autoTargetLandingPoint.y,
                        player.transform.position.z
                    );
                }
                else
                {
                    Vector2 boxSize = new(player.BoxCollider.bounds.size.x * 0.99f, 0.1f);
                    Vector2 origin = (Vector2)player.BoxCollider.bounds.center + Vector2.down * player.BoxCollider.bounds.extents.y;
                    RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, Mathf.Infinity, player.GroundLayer);

                    if (hit.collider)
                        player.transform.position = hit.point;
                }

                player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                player.Rigidbody.linearVelocity = Vector2.zero;
                phase = SkillPhase.WaitingForLanding;
            }

            return;
        }

        if ((phase == SkillPhase.Moving || phase == SkillPhase.WaitingForLanding) &&
            !forceGroundedIgnore &&
            player.isGrounded &&
            !landingTriggered)
        {
            player.Rigidbody.linearVelocity = Vector2.zero;
            landingTriggered = true;

            player.SetEffectState(PlayerEffectState.SpearAirSkill);
            if (phase == SkillPhase.Moving)
                player.Animator.Play("Spear_Ground_Land");
            else
                player.Animator.Play("Spear_Flying_Land");

            ApplyLandingDamage();
            CameraUtility.ShakeCamera(
                duration: 0.3f,
                strength: 0.5f,
                vibrato: 10,
                randomness: 90,
                fadeOut: true
            );
            phase = SkillPhase.Landing;
            player.PlayClip(player.SpearSkill);
        }

        if (phase == SkillPhase.Landing)
        {
            AnimatorStateInfo animInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
            if ((animInfo.IsName("Spear_Ground_Land") || animInfo.IsName("Spear_Flying_Land")) && animInfo.normalizedTime >= 1f)
                stateMachine.ChangeState(new LocomotionState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        if (phase != SkillPhase.Moving || useAutoTargetTrajectory)
            return;

        if (mode == SkillMode.Ground || mode == SkillMode.LowAir)
        {
            Vector2 velocity = player.Rigidbody.linearVelocity;
            velocity.x = player.facingDirection * dashSpeed;
            player.Rigidbody.linearVelocity = velocity;
        }
    }

    private System.Collections.IEnumerator ResetForceGroundedIgnore()
    {
        yield return new WaitForSeconds(0.1f);
        forceGroundedIgnore = false;
    }

    private bool TryResolveAutoTargetLandingPoint(out Vector2 landingPoint)
    {
        landingPoint = default;
        if (spearData.spearSkillAutoTargetRange <= 0f)
            return false;

        if (!CombatTargetingUtility.TryFindNearestEnemyPoint(
                player.transform.position,
                spearData.spearSkillAutoTargetRange,
                LayerMask.GetMask("Enemy"),
                out Vector2 enemyPoint))
        {
            return false;
        }

        Vector2 boxSize = new(player.BoxCollider.bounds.size.x * 0.99f, 0.1f);
        if (CombatTargetingUtility.TryProjectPointToGround(enemyPoint, boxSize, player.GroundLayer, 5f, 30f, out landingPoint))
            return true;

        landingPoint = enemyPoint;
        return true;
    }

    private bool TryLaunchToTarget(Vector2 targetPoint)
    {
        float gravityMagnitude = Mathf.Abs(Physics2D.gravity.y * player.Rigidbody.gravityScale);
        if (!CombatTargetingUtility.TryGetBallisticVelocity(
                player.transform.position,
                targetPoint,
                gravityMagnitude,
                spearData.spearSkillAutoTargetArcHeight,
                out Vector2 velocity))
        {
            return false;
        }

        player.Rigidbody.linearVelocity = velocity;
        return true;
    }

    private void FaceTarget(float targetX)
    {
        float deltaX = targetX - player.transform.position.x;
        if (Mathf.Abs(deltaX) <= 0.01f)
            return;

        player.facingDirection = deltaX > 0f ? 1 : -1;
        player.transform.rotation = Quaternion.Euler(0f, player.facingDirection == -1 ? 180f : 0f, 0f);
    }

    private void ApplyLandingDamage()
    {
        Vector2 center = player.transform.position;
        if (phase == SkillPhase.Moving)
            center += new Vector2(player.facingDirection * 0.5f, 0f);

        float radius = spearData.spearSkillRange;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, LayerMask.GetMask("Enemy"));
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent<Monster>(out Monster monster))
                continue;

            monster.TakeDamage(spearData.spearSkillDamage);
            monster.KnockBack(
                attacker: player.transform,
                knockBackForce: 20f,
                knockBackAngle: 45f,
                duration: 0.6f
            );
        }

        DebugDrawCrossX(center, radius, 0.3f);
    }

    private void DebugDrawCrossX(Vector2 center, float radius, float duration)
    {
#if UNITY_EDITOR
        Color color = Color.red;
        Debug.DrawLine(center + new Vector2(-radius, 0f), center + new Vector2(radius, 0f), color, duration);
        Debug.DrawLine(center + new Vector2(0f, -radius), center + new Vector2(0f, radius), color, duration);
#endif
    }
}