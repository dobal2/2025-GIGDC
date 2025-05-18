using UnityEngine;

public class SpearSkillState : PlayerState
{
    private bool hasLanded = false;
    private Vector2 direction;
    private float horizontalForce;
    private float verticalForce;

    public SpearSkillState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "SpearSkill";
    public override bool IsCombatState => true;

    public override void Enter()
    {
        if (player.isGrounded)
        {
            direction = new Vector2(player.facingDirection, 1f).normalized;
            horizontalForce = 10f;
            verticalForce = 5f;
        }
        else
        {
            direction = Vector2.down;
            horizontalForce = 0f;
            verticalForce = 20f;
        }

        player.Rigidbody.linearVelocity = direction * new Vector2(horizontalForce, verticalForce);
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
        // ��: �ٴ� ��� ���� ����
    }
}