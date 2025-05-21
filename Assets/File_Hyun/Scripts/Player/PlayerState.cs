using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;

    public virtual bool IsCombatState => false;

    public abstract string Name { get; }

    public PlayerState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual bool CanTransitionTo(PlayerState newState) => true;

    protected bool TryHandleSkillInput()
    {
        if (player.skillRequested && player.AttackController.CanUseSkill)
        {
            player.ConsumeSkillRequest();
            var skillState = player.AttackController.GetSkillState(stateMachine);
            if (skillState != null)
            {
                stateMachine.ChangeState(skillState);
                return true;
            }
        }
        return false;
    }

    protected bool TryHandleComboContinue()
    {
        if (player.AttackBuffered &&
            player.AttackController.CanComboInput &&
            (player.isGrounded || player.AttackController.CanStartAirborneCombo) &&
            player.AttackController.ComboStep > 0)
        {
            player.ConsumeAttackBuffer();
            player.AttackController.MarkComboInputReceived();
            player.AttackController.ContinueCombo();
            stateMachine.ChangeState(new GenericAttackState(player, stateMachine));
            return true;
        }
        return false;
    }

    protected bool TryHandleComboStart()
    {
        if (player.AttackBuffered &&
            (player.isGrounded || player.AttackController.CanStartAirborneCombo) &&
            player.AttackController.ComboStep == 0)
        {
            player.ConsumeAttackBuffer();
            player.AttackController.StartCombo();
            stateMachine.ChangeState(new GenericAttackState(player, stateMachine));
            return true;
        }
        return false;
    }

    protected bool TryHandleDash()
    {
        if (player.dashBufferTimer > 0f &&
            Time.time >= player.lastDashTime + player.DashCooldown)
        {
            if (player.isGrounded || player.canAirDash)
            {
                stateMachine.ChangeState(new PlayerDashState(player, stateMachine));
                return true;
            }
        }
        return false;
    }
}
