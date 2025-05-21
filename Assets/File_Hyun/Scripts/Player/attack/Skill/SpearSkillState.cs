using UnityEngine;

public class SpearSkillState : PlayerState
{
    private bool hasLanded = false;
    private Vector2 initialVelocity;

    public SpearSkillState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "SpearSkill";
    public override bool IsCombatState => true;

    public override void Enter()
    {
        if (player.isGrounded)
        {
            Vector2 direction = new(player.facingDirection, 1f);
            initialVelocity = direction.normalized * 10f;
        }
        else
        {
            initialVelocity = Vector2.down * 15f;
        }

        player.Rigidbody.linearVelocity = initialVelocity;

        player.jumpBufferTimer = 0f;
        player.dashBufferTimer = 0f;
        player.attackBufferTimer = 0f;
        player.skillRequested = false;
        player.isJumping = false;
    }

    public override void Update()
    {
        if (!hasLanded && player.isGrounded)
        {
            hasLanded = true;
            TriggerImpactEffect();
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    void TriggerImpactEffect()
    {
        
    }
}