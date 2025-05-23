using UnityEngine;

public class SpearSkillState : PlayerState
{
    private enum SkillPhase { Charging, Moving, WaitingForLanding, Landing }
    private enum SkillMode { Ground, LowAir, HighAir }

    private SkillMode mode;
    private SkillPhase phase;

    private float timer;
    private float dashSpeed;
    private float jumpSpeed = 10f;
    private float chargeDuration = 0.5f;
    private float landingDuration = 0.6f;

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
            mode = SkillMode.Ground;
            phase = SkillPhase.Moving;
            dashSpeed = 15f;
            Vector2 vel = new Vector2(player.facingDirection * dashSpeed, jumpSpeed);
            player.Rigidbody.linearVelocity = vel;
        }
        else if (player.isLowAir)
        {
            mode = SkillMode.LowAir;
            phase = SkillPhase.Moving;
            dashSpeed = 30f;
            Vector2 vel = new Vector2(player.facingDirection * dashSpeed, 0f);
            player.Rigidbody.linearVelocity = vel;
        }
        else
        {
            mode = SkillMode.HighAir;
            phase = SkillPhase.Charging;
            player.Rigidbody.linearVelocity = Vector2.zero;
            player.Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX |
                                           RigidbodyConstraints2D.FreezePositionY;
        }

        forceGroundedIgnore = true;
        timer = 0f;
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
                Vector2 boxSize = new Vector2(player.BoxCollider.bounds.size.x * 0.99f, 0.1f);
                Vector2 origin = player.BoxCollider.bounds.center;

                RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, 100f, player.GroundLayer);

                if (hit.collider)
                {
                    float yOffset = player.BoxCollider.bounds.extents.y;
                    player.transform.position = hit.point + Vector2.up * yOffset;
                }

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

    public override void Exit()
    {
        player.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        player.Rigidbody.linearVelocity = Vector2.zero;
    }
}