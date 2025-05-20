using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AttackController : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponDatabase;

    private PlayerController player;
    private WeaponData currentWeaponData;

    public WeaponType CurrentWeapon { get; private set; }

    private int comboStep = 0;
    private float pushTimer = 0f;
    private float comboDelayTimer = 0f;
    private float comboKeepTimer = 0f;

    private float currentPushDistance = 0f;
    private float pushSpeedPerSecond = 0f;

    private bool receivedNextInput = false;

    public bool HasReachedMaxCombo => comboStep >= currentWeaponData.maxComboCount;
    public bool IsPushing => pushTimer > 0f && pushSpeedPerSecond != 0f;
    public bool IsInComboDelay => comboDelayTimer > 0f;
    public bool CanMove => !IsPushing && !IsInComboDelay;
    public bool CanComboInput => !IsPushing && !IsInComboDelay;

    public bool ShouldEndCombo =>
        comboStep > 0 &&
        !receivedNextInput &&
        !IsPushing &&
        !IsInComboDelay &&
        comboKeepTimer <= 0f;

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
        currentWeaponData = weaponDatabase.GetData(weapon);

        comboStep = 0;
        pushTimer = 0f;
        comboDelayTimer = 0f;
        comboKeepTimer = 0f;
        currentPushDistance = 0f;
        pushSpeedPerSecond = 0f;
        receivedNextInput = false;
    }

    public void StartCombo()
    {
        comboStep = 1;
        receivedNextInput = false;

        PlayCombo(comboStep);
    }

    public void ContinueCombo()
    {
        if (HasReachedMaxCombo)
            return;

        if (!CanComboInput)
            return;

        comboStep++;
        receivedNextInput = false;

        PlayCombo(comboStep);
    }

    public void MarkComboInputReceived()
    {
        receivedNextInput = true;
    }

    public void UpdateComboTimer()
    {
        if (pushTimer > 0f)
        {
            pushTimer -= Time.deltaTime;
        }
        else if (comboDelayTimer > 0f)
        {
            comboDelayTimer -= Time.deltaTime;
        }
        else if (comboKeepTimer > 0f)
        {
            comboKeepTimer -= Time.deltaTime;
        }
    }

    public float GetPushDelta(float deltaTime)
    {
        return pushSpeedPerSecond * deltaTime;
    }

    private void PlayCombo(int step)
    {
        Debug.Log($"[Attack] 현재 콤보 단계: {step}");

        currentPushDistance = currentWeaponData.GetPush(step);
        pushTimer = currentWeaponData.GetDelay(step);
        comboDelayTimer = currentWeaponData.GetComboDelay(step);
        comboKeepTimer = currentWeaponData.comboInfos[step - 1].ComboKeep;

        if (pushTimer <= 0f)
        {
            pushSpeedPerSecond = 0f;
        }
        else
        {
            pushSpeedPerSecond = currentPushDistance / pushTimer;
        }
    }

    public PlayerState GetAttackState(PlayerStateMachine stateMachine)
    {
        return CurrentWeapon switch
        {
            WeaponType.Spear => new SpearAttackState(player, stateMachine),
            _ => null
        };
    }

    public PlayerState GetSkillState(PlayerStateMachine stateMachine)
    {
        return CurrentWeapon switch
        {
            WeaponType.Spear => new SpearSkillState(player, stateMachine),
            _ => null
        };
    }
}