using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AttackController : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponDatabase;

    private PlayerController player;
    private WeaponData currentWeaponData;

    public WeaponType CurrentWeapon { get; private set; }
    public bool HasReachedMaxCombo => comboStep >= currentWeaponData.maxComboCount;

    private int comboStep = 0;
    private float comboDelayTimer = 0f;

    private float currentPushDistance = 0f;
    private float pushSpeedPerSecond = 0f;

    private bool receivedNextInput = false;

    public bool CanComboInput => comboDelayTimer <= 0f;

    public bool ShouldEndCombo =>
        comboStep > 0 &&
        (HasReachedMaxCombo || !receivedNextInput) &&
        CanComboInput;

    public bool IsPushing => comboDelayTimer > 0f && pushSpeedPerSecond > 0f;

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
        comboDelayTimer = 0f;
        currentPushDistance = 0f;
        pushSpeedPerSecond = 0f;
        receivedNextInput = false;
    }

    public void StartCombo()
    {
        comboStep = 1;
        receivedNextInput = false;

        PlayCombo(comboStep);
        comboDelayTimer = GetComboDelay(comboStep);
    }

    public void ContinueCombo()
    {
        if (comboStep >= currentWeaponData.maxComboCount)
            return;

        if (!CanComboInput)
            return;

        comboStep++;
        receivedNextInput = false;

        PlayCombo(comboStep);
        comboDelayTimer = GetComboDelay(comboStep);
    }

    public void MarkComboInputReceived()
    {
        receivedNextInput = true;
    }

    public void UpdateComboTimer()
    {
        if (comboDelayTimer > 0f)
            comboDelayTimer -= Time.deltaTime;
    }

    public float GetPushDelta(float deltaTime)
    {
        return pushSpeedPerSecond * deltaTime;
    }

    private void PlayCombo(int step)
    {
        Debug.Log($"[Attack] 현재 콤보 단계: {step}");

        currentPushDistance = GetComboPush(step);
        float delay = GetComboDelay(step);

        if (delay <= 0f)
        {
            pushSpeedPerSecond = 0f;
        }
        else
        {
            pushSpeedPerSecond = currentPushDistance / delay;
        }
    }

    private float GetComboPush(int step)
    {
        if (currentWeaponData.comboPushDistances != null &&
            currentWeaponData.comboPushDistances.Length >= step)
            return currentWeaponData.comboPushDistances[step - 1];

        return 0f;
    }

    private float GetComboDelay(int step)
    {
        if (currentWeaponData.comboDelays != null &&
            currentWeaponData.comboDelays.Length >= step)
            return currentWeaponData.comboDelays[step - 1];

        return 0.1f;
    }

    public PlayerState GetAttackState(PlayerStateMachine stateMachine)
    {
        return CurrentWeapon switch
        {
            WeaponType.Spear => new SpearAttackState(player, stateMachine),
            //WeaponType.Bow => new BowAttackState(player, stateMachine),
            //WeaponType.Unplanned => new UnplannedAttackState(player, stateMachine),
            _ => null
        };
    }

    public PlayerState GetSkillState(PlayerStateMachine stateMachine)
    {
        return CurrentWeapon switch
        {
            WeaponType.Spear => new SpearSkillState(player, stateMachine),
            //WeaponType.Bow => new BowSkillState(player, stateMachine),
            //WeaponType.Unplanned => new UnplannedSkillState(player, stateMachine),
            _ => null
        };
    }
}