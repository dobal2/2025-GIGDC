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
    private bool landingTriggered = false;
    private bool forceGroundedIgnore = false;
    private bool skillStarted;
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
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.isNoClip = true;

        if (player.isGrounded)
        {
            player.Animator.Play("Spear_Ground_Jump");
            mode = SkillMode.Ground;
            phase = SkillPhase.Moving;
            dashSpeed = 14f;
            Vector2 vel = new(player.facingDirection * dashSpeed, spearData.jumpSpeed);
            player.Rigidbody.linearVelocity = vel;
        }
        else if (player.isLowAir)
        {
            player.Animator.Play("Spear_GroundAir_Jump");
            mode = SkillMode.LowAir;
            phase = SkillPhase.Moving;
            dashSpeed = 18f;
            Vector2 vel = new(player.facingDirection * dashSpeed, spearData.jumpSpeed * 0.5f);
            player.Rigidbody.linearVelocity = vel;
        }
        else
        {
            player.Animator.Play("Spear_Flying_Charge");
            mode = SkillMode.HighAir;
            phase = SkillPhase.Charging;
            player.Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX |
                                           RigidbodyConstraints2D.FreezePositionY;
        }

        forceGroundedIgnore = true;
        landingTriggered = false;
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
                Vector2 boxSize = new(player.BoxCollider.bounds.size.x * 0.99f, 0.1f);
                Vector2 origin = (Vector2)player.BoxCollider.bounds.center + Vector2.down * player.BoxCollider.bounds.extents.y;
                RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, Mathf.Infinity, player.GroundLayer);

                if (hit.collider)
                    player.transform.position = hit.point;

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
            else if (phase == SkillPhase.WaitingForLanding)
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
        if (phase == SkillPhase.Moving && (mode == SkillMode.Ground || mode == SkillMode.LowAir))
        {
            Vector2 vel = player.Rigidbody.linearVelocity;
            vel.x = player.facingDirection * dashSpeed;
            player.Rigidbody.linearVelocity = vel;
        }
    }

    private System.Collections.IEnumerator ResetForceGroundedIgnore()
    {
        yield return new WaitForSeconds(0.1f);
        forceGroundedIgnore = false;
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
            if (hit.TryGetComponent<Monster>(out Monster monster))
            {
                monster.TakeDamage(spearData.spearSkillDamage);
                monster.KnockBack(
                    attacker: player.transform,
                    knockBackForce: 20,
                    knockBackAngle: 45,
                    duration: 0.6f
                );
            }
        }

        DebugDrawCrossX(center, radius, 0.3f);
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