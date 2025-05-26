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
    private float lastSkillTime = -999f;
    private bool receivedNextInput = false;
    private bool airborneComboUsed = false;

    public bool HasReachedMaxCombo => comboStep >= currentWeaponData.MaxComboCount;
    public bool IsPushing => pushTimer > 0f && pushSpeedPerSecond != 0f;
    public bool IsInComboDelay => comboDelayTimer > 0f;
    public bool CanMove => !IsPushing && !IsInComboDelay;
    public bool CanComboInput => !IsPushing && !IsInComboDelay;
    public bool CanStartAirborneCombo => !player.isGrounded && !airborneComboUsed;
    public int ComboStep => comboStep;
    public bool CanUseSkill => Time.time >= lastSkillTime + currentWeaponData.skillcooldown;

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
        receivedNextInput = false;
        airborneComboUsed = false;
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
        {
            comboStep = 0;
            pushTimer = 0f;
            comboDelayTimer = 0f;
            comboKeepTimer = 0f;
            currentPushDistance = 0f;
            pushSpeedPerSecond = 0f;
            receivedNextInput = false;
            if(player.isGrounded)
                airborneComboUsed = false;
            return;
        }

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

    public void CancelPush()
    {
        if (comboStep < 1 || comboStep > currentWeaponData.MaxComboCount)
            return;

        pushTimer = 0f;
        pushSpeedPerSecond = 0f;
        currentPushDistance = 0f;

        comboDelayTimer = 0f;
        comboKeepTimer = currentWeaponData.comboInfos[comboStep - 1].ComboKeep;
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
                {
                    ResetCombo();
                }
            }
        }
    }

    public float GetPushDelta(float deltaTime)
    {
        return pushSpeedPerSecond * deltaTime;
    }

    private string GetComboAnimName(int step)
    {
        string baseName;
        baseName = $"{step}";

        if (!player.isGrounded)
            baseName = "Flying_" + baseName;

        return baseName;
    }

    private void PlayCombo(int step)
    {
        Debug.Log($"[Attack] 현재 콤보 단계: {step}");

        currentPushDistance = currentWeaponData.GetPush(step);
        pushTimer = currentWeaponData.GetDelay(step);
        comboDelayTimer = currentWeaponData.GetComboDelay(step);
        comboKeepTimer = currentWeaponData.comboInfos[step - 1].ComboKeep;

        if (!player.isGrounded && currentWeaponData.MaxComboCount == step)
            airborneComboUsed = true;

        if (pushTimer <= 0f)
        {
            pushSpeedPerSecond = 0f;
        }
        else
        {
            pushSpeedPerSecond = currentPushDistance / pushTimer;
        }

        player.Animator.Play(GetComboAnimName(step));
    }

    public void ResetAirborneCombo()
    {
        airborneComboUsed = false;
    }

    public void OnAttackEnter()
    {
        if (comboStep < 1 || comboStep > currentWeaponData.MaxComboCount)
            return;

        if (CurrentWeapon == WeaponType.Bow || CurrentWeapon == WeaponType.Bomb)
            FireProjectile(comboStep);
    }

    public void OnAttackExit()
    {
        // 필요 시 여기에 투사체 관련 정리, 애니메이션 종료 등 추가 가능
    }

    private void FireProjectile(int step)
    {
        var data = currentWeaponData.comboInfos[step - 1];
        if (data.projectilePrefab == null)
            return;

        Vector3 spawnPos = player.transform.position + new Vector3(player.facingDirection * 0.5f, 0, 0);
        GameObject obj = GameObject.Instantiate(data.projectilePrefab, spawnPos, Quaternion.identity);

        if (obj.TryGetComponent(out Projectile projectile))
            projectile.Initialize(player.facingDirection);
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