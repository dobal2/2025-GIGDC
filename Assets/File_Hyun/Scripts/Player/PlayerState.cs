using UnityEngine;

public enum PlayerStateType
{
    Missing, // isfucked = true;
    Locomotion,
    Dash,
    SpearAttack,
    SpearSkill,
    BowAttack,
    BowSkill,
    BombAttack,
    BombSkill,
    Death,
    // 필요 시 추가
}

public abstract class PlayerState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;

    public abstract PlayerStateType StateType { get; }

    public virtual bool IsCombatState => false;

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

    protected bool TryHandleDash()
    {
        if (player.DashPressed && Time.time >= player.lastDashTime + player.DashCooldown)
        {
            if (player.isGrounded || player.canAirDash)
            {
                player.DashPressed = false;
                stateMachine.ChangeState(new DashState(player, stateMachine));
                return true;
            }
        }
        return false;
    }

    protected bool TryHandleSkillInput()
    {
        if (player.SkillPressed && player.AttackController.CanUseSkill)
        {
            player.SkillPressed = false;
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
            if (player.AttackController.CurrentWeapon == WeaponType.Bomb &&
                !player.AttackController.CanThrowBomb())
                return false;

            player.ConsumeAttackBuffer();
            player.AttackController.ContinueCombo();
            stateMachine.ChangeState(player.AttackController.GetAttackState(stateMachine));
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
            if (player.AttackController.CurrentWeapon == WeaponType.Bomb &&
                !player.AttackController.CanThrowBomb())
                return false;

            player.ConsumeAttackBuffer();
            player.AttackController.StartCombo();
            stateMachine.ChangeState(player.AttackController.GetAttackState(stateMachine));
            return true;
        }
        return false;
    }
}
