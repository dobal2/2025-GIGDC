using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AttackController : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponDatabase;
    [SerializeField] private float comboMaxTime = 0.5f;

    private PlayerController player;
    private WeaponData currentWeaponData;

    public WeaponType CurrentWeapon { get; private set; }
    public bool HasReachedMaxCombo => comboStep >= currentWeaponData.maxComboCount;
    private int comboStep = 0;
    private float comboTimer = 0f;

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
        comboTimer = 0f;
    }

    public void StartCombo()
    {
        comboStep = 1;
        comboTimer = comboMaxTime;
        PlayCombo(comboStep);
    }

    public void ContinueCombo()
    {
        if (comboStep >= currentWeaponData.maxComboCount)
            return;

        comboStep++;
        comboTimer = comboMaxTime;
        PlayCombo(comboStep);
    }

    public void UpdateComboTimer()
    {
        if (comboTimer > 0f)
            comboTimer -= Time.deltaTime;
    }

    public bool IsComboTimedOut => comboTimer <= 0f;

    void PlayCombo(int step)
    {
        Debug.Log($"[Attack] 현재 콤보 단계: {step}");
        // 애니메이션 재생, 피격 판정 생성, 이펙트 등
        float push = GetComboPush(step);
        Vector2 velocity = new(push * player.facingDirection, player.Rigidbody.linearVelocity.y);
        player.Rigidbody.linearVelocity = velocity;
    }

    float GetComboPush(int step)
    {
        if (currentWeaponData.comboPushDistances != null &&
            currentWeaponData.comboPushDistances.Length >= step)
            return currentWeaponData.comboPushDistances[step - 1];

        return 0f;
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