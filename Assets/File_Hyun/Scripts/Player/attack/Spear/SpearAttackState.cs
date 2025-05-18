using UnityEngine;

public class SpearAttackState : PlayerState
{
    private bool waitingForNextInput;
    private bool comboEnded;

    public SpearAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override string Name => "SpearAttack";
    public override bool IsCombatState => true;

    public override void Enter()
    {
        waitingForNextInput = true;
        comboEnded = false;

        player.AttackController.StartCombo();
    }

    public override void Update()
    {
        player.AttackController.UpdateComboTimer();

        if (player.skillRequested)
        {
            player.ConsumeSkillRequest();
            stateMachine.ChangeState(player.AttackController.GetSkillState(stateMachine));
            return;
        }

        if (comboEnded || player.AttackController.IsComboTimedOut)
        {
            player.ConsumeAttackBuffer();
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
            return;
        }

        if (waitingForNextInput && player.AttackBuffered)
        {
            player.ConsumeAttackBuffer();
            player.AttackController.ContinueCombo();

            if (player.AttackController.HasReachedMaxCombo)
            {
                comboEnded = true;
                waitingForNextInput = false;
            }
        }
    }
}