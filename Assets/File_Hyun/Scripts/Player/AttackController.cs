using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AttackController : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponDatabase;

    private PlayerController player;

    [HideInInspector] public SpearData spearData;
    [HideInInspector] public BowData bowData;
    [HideInInspector] public BombData bombData;

    public WeaponType CurrentWeapon { get; private set; }

    private int comboStep = 0;
    private float pushTimer = 0f;
    private float comboDelayTimer = 0f;
    private float comboKeepTimer = 0f;
    private float currentPushDistance = 0f;
    private float pushSpeedPerSecond = 0f;
    private float lastSkillTime = -999f;
    private bool airborneComboUsed = false;

    public bool HasReachedMaxCombo => comboStep >= GetMaxCombo();
    public bool IsPushing =>
    pushTimer > 0f &&
    pushSpeedPerSecond != 0f &&
    (
        !player.isEdge ||
        (pushSpeedPerSecond * player.facingDirection < 0 && player.isGroundedLeft) ||
        (pushSpeedPerSecond * player.facingDirection > 0 && player.isGroundedRight)
    );
    public bool IsInComboDelay => comboDelayTimer > 0f;
    public bool CanMove => !IsPushing && !IsInComboDelay;
    public bool CanComboInput => !IsPushing && !IsInComboDelay;
    public bool CanStartAirborneCombo => !player.isGrounded && !airborneComboUsed;
    public int ComboStep => comboStep;
    public bool CanUseSkill => Time.time >= lastSkillTime + GetSkillCooldown();

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void Initialize(WeaponType initialWeapon)
    {
        SetWeapon(initialWeapon);
    }

    public void SetWeapon(WeaponType weapon)
    {
        CurrentWeapon = weapon;

        switch (weapon)
        {
            case WeaponType.Spear:
                spearData = weaponDatabase.GetData<SpearData>(WeaponType.Spear);
                player.Animator.runtimeAnimatorController = spearData.animatorController;
                break;

            case WeaponType.Bow:
                bowData = weaponDatabase.GetData<BowData>(WeaponType.Bow);
                player.Animator.runtimeAnimatorController = bowData.animatorController;
                break;

                // case WeaponType.Bomb:
                //     bombData = weaponDatabase.GetData<BombData>(WeaponType.Bomb);
                //     player.Animator.runtimeAnimatorController = bombData.animatorController;
                //     break;
        }

        ResetCombo();
    }

    public void MarkSkillUsed()
    {
        lastSkillTime = Time.time;
    }

    public void ResetCombo()
    {
        comboStep = 0;
        pushTimer = 0f;
        comboDelayTimer = 0f;
        comboKeepTimer = 0f;
        currentPushDistance = 0f;
        pushSpeedPerSecond = 0f;
        airborneComboUsed = false;
    }

    public void StartCombo()
    {
        comboStep = 1;

        PlayCombo(comboStep);
    }

    public void ContinueCombo()
    {
        if (HasReachedMaxCombo)
        {
            comboStep = 0;
            pushTimer = 0f;
            comboDelayTimer = 0f;
            comboKeepTimer = 0f;
            currentPushDistance = 0f;
            pushSpeedPerSecond = 0f;
            if (player.isGrounded)
                airborneComboUsed = false;
            return;
        }

        if (!CanComboInput)
            return;

        comboStep++;

        PlayCombo(comboStep);
    }

    public void CancelPush()
    {
        if (comboStep < 1 || comboStep > GetMaxCombo())
            return;

        pushTimer = 0f;
        pushSpeedPerSecond = 0f;
        currentPushDistance = 0f;

        comboDelayTimer = 0f;
        comboKeepTimer = GetComboKeep(comboStep);
    }

    public void UpdateComboTimer()
    {
        if (pushTimer > 0f)
        {
            pushTimer -= Time.deltaTime;
            return;
        }

        if (comboDelayTimer > 0f)
        {
            comboDelayTimer -= Time.deltaTime;
            return;
        }

        if (comboKeepTimer > 0f)
        {
            if (player.isGrounded)
            {
                comboKeepTimer -= Time.deltaTime;
                if (comboKeepTimer <= 0f)
                    ResetCombo();
            }
        }
    }

    public float GetPushDelta(float deltaTime)
    {
        return pushSpeedPerSecond * deltaTime;
    }

    private void PlayCombo(int step)
    {
        Debug.Log($"[Attack] 현재 콤보 단계: {step}");

        switch (CurrentWeapon)
        {
            case WeaponType.Spear:
                currentPushDistance = spearData.GetPush(step);
                pushTimer = spearData.GetDelay(step);
                comboDelayTimer = spearData.GetComboDelay(step);
                comboKeepTimer = spearData.GetComboKeep(step);
                if (!player.isGrounded && spearData.MaxCombo == step)
                    airborneComboUsed = true;
                break;

            case WeaponType.Bow:
                currentPushDistance = bowData.GetPush(step);
                pushTimer = bowData.GetDelay(step);
                comboDelayTimer = bowData.GetComboDelay(step);
                comboKeepTimer = bowData.GetComboKeep(step);
                if (!player.isGrounded && bowData.MaxCombo == step)
                    airborneComboUsed = true;
                break;

             //case WeaponType.Bomb:

        }

        pushSpeedPerSecond = pushTimer > 0f ? currentPushDistance / pushTimer : 0f;

        string animName = !player.isGrounded ? $"Flying_{step}" : $"{step}";
        player.Animator.Play(animName);
    }

    public void ResetAirborneCombo()
    {
        airborneComboUsed = false;
    }

    public PlayerState GetSkillState(PlayerStateMachine stateMachine)
    {
        return CurrentWeapon switch
        {
            WeaponType.Spear => new SpearSkillState(player, stateMachine),
            WeaponType.Bow => new BowSkillState(player, stateMachine),
            // WeaponType.Bomb => new BombSkillState(player, stateMachine),
            _ => null
        };
    }

    public PlayerState GetAttackState(PlayerStateMachine stateMachine)
    {
        return CurrentWeapon switch
        {
            WeaponType.Spear => new SpearAttackState(player, stateMachine),
            WeaponType.Bow => new BowAttackState(player, stateMachine),
            // WeaponType.Bomb => new BombAttackState(player, stateMachine),
            _ => null
        };
    }

    private int GetMaxCombo()
    {
        return CurrentWeapon switch
        {
            WeaponType.Spear => spearData.MaxCombo,
            WeaponType.Bow => bowData.MaxCombo,
            // WeaponType.Bomb => bombData.MaxCombo,
            _ => 0
        };
    }

    private float GetSkillCooldown()
    {
        return CurrentWeapon switch
        {
            WeaponType.Spear => spearData.spearSkillcooldown,
            WeaponType.Bow => bowData.bowSkillcooldown,
            // WeaponType.Bomb => bombData.Bombskillcooldown,
            _ => 1f
        };
    }

    private float GetComboKeep(int step)
    {
        return CurrentWeapon switch
        {
            WeaponType.Spear => spearData.GetComboKeep(step),
            WeaponType.Bow => bowData.GetComboKeep(step),
            // WeaponType.Bomb => bombData.GetComboKeep(step),
            _ => 0f
        };
    }
}