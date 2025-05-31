using UnityEngine;
using static PlayerController;

public class SpearSkillState : PlayerState
{
    private enum SkillPhase { Charging, Moving, WaitingForLanding, Landing }
    private enum SkillMode { Ground, LowAir, HighAir }

    private SkillMode mode;
    private SkillPhase phase;

    private float timer;
    private float dashSpeed;
    private float jumpSpeed = 16f;
    private float chargeDuration = 0.6f;
    private float landingDuration = 0.83f;

    private bool landingTriggered = false;
    private bool forceGroundedIgnore = false;

    public SpearSkillState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override PlayerStateType StateType => PlayerStateType.SpearSkill;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.AttackController.MarkSkillUsed();
        Debug.Log("[Skill] 스킬 사용됨 - 쿨타임 시작");

        if (player.isGrounded)
        {
            player.Animator.Play("Spear_Ground_Jump");
            mode = SkillMode.Ground;
            phase = SkillPhase.Moving;
            dashSpeed = 20f;
            Vector2 vel = new(player.facingDirection * dashSpeed, jumpSpeed);
            player.Rigidbody.linearVelocity = vel;
        }
        else if (player.isLowAir)
        {
            player.Animator.Play("Spear_GroundAir_Jump");
            mode = SkillMode.LowAir;
            phase = SkillPhase.Moving;
            dashSpeed = 30f;
            Vector2 vel = new(player.facingDirection * dashSpeed, jumpSpeed * 0.5f);
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
        timer = 0f;
    }

    public override void Exit()
    {
        player.SetEffectState(PlayerEffectState.None);
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        player.Rigidbody.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer > 0.1f)
            forceGroundedIgnore = false;

        if (phase == SkillPhase.Charging)
        {
            if (timer >= chargeDuration)
            {
                Vector2 boxSize = new(player.BoxCollider.bounds.size.x * 0.99f, 0.1f);
                Vector2 origin = player.BoxCollider.bounds.center;

                RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, 100f, player.GroundLayer);

                if (hit.collider) player.transform.position = hit.point;

                player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                player.Rigidbody.linearVelocity = Vector2.zero;
                phase = SkillPhase.WaitingForLanding;
            }

            return;
        }

        if ((phase == SkillPhase.Moving || phase == SkillPhase.WaitingForLanding)
            && !forceGroundedIgnore && player.isGrounded && !landingTriggered)
        {
            player.Rigidbody.linearVelocity = Vector2.zero;
            landingTriggered = true;

            player.SetEffectState(PlayerEffectState.SpearAirSkill);
            if (phase == SkillPhase.Moving)
                player.Animator.Play("Spear_Ground_Land");
            else if (phase == SkillPhase.WaitingForLanding)
                player.Animator.Play("Spear_Flying_Land");

            phase = SkillPhase.Landing;
            timer = 0f;

            // TODO: 착지 이펙트, 광역 타격 처리
        }

        if (phase == SkillPhase.Landing && timer >= landingDuration)
        {
            stateMachine.ChangeState(new PlayerLocomotionState(player, stateMachine));
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
}