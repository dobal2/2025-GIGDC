using UnityEngine;

public class SpearAttackState : PlayerState
{
    private int comboStep = 0;
    private float comboTimer = 0f;
    private readonly float comboMaxTime = 0.5f;

    public SpearAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "SpearAttack";
    public override bool IsCombatState => true;

    public override void Enter()
    {
        comboStep = 1;
        comboTimer = comboMaxTime;
        PlayAttackAnimation(comboStep);
    }

    public override void Update()
    {
        comboTimer -= Time.deltaTime;

        if (comboTimer <= 0f)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
            return;
        }

        if (Input.GetKeyDown(InputManager.Instance.keyData.Player.AttackKey) && comboStep < 3)
        {
            comboStep++;
            comboTimer = comboMaxTime;
            PlayAttackAnimation(comboStep);
        }
    }

    public override void FixedUpdate()
    {
        ApplyForwardMotion(comboStep);
    }

    void PlayAttackAnimation(int step)
    {
        // ��: Animator.Play($"SpearAttack{step}");
    }

    void ApplyForwardMotion(int step)
    {
        float forwardForce = 2f + 0.5f * (step - 1);
        player.Rigidbody.linearVelocity = new Vector2(forwardForce * player.facingDirection, player.Rigidbody.linearVelocity.y);
    }
}