using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public AttackController AttackController { get; private set; }
    public Animator Animator { get; private set; }

    public enum PlayerEffectState
    {
        None,
        GroundWalkDust,
        Dash,
        FastFall,
        SpearAirSkill,
        BowSkillCharging,
        BowSkillRelease,
        BowSkillFullChargeRelease,
        Dying,
    }

    public event Action<PlayerEffectState> OnEffectStateChanged;
    public event Action<WeaponType> OnChangeWeapon;
    public event Action OnChainAttackFinished;
    public event Action<bool> OnCounterTry;
    public event Action<int, int> OnEnergyChanged;

    public PlayerEffectState CurrentEffectState { get; private set; }
    public float MoveInput { get; set; }
    public bool DownHeld { get; set; }
    public bool JumpHeld { get; set; }
    public bool DashPressed { get; set; }
    public bool SkillPressed { get; set; }
    public bool SkillHeld { get; set; }
    public bool ChangePressed { get; set; }
    public bool CounterPressed { get; set; }
    public int CurrentEnergy { get; private set; }

    public bool JumpPressed
    {
        set
        {
            if (value)
                jumpBufferTimer = jumpBufferTime;
        }
    }

    public bool AttackPressed
    {
        set
        {
            if (value)
                attackBufferTimer = attackBufferTime;
        }
    }

    public PlayerStateType CurrentStateType => stateMachine.CurrentStateType;
    public bool CanTakeDamage => !isNoClip && !PlayerHealth.Instance.isInvincible;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float fastFallGravityScale = 20f;
    public float MoveSpeed => moveSpeed;
    public Vector2 CurrentVelocity => rb.linearVelocity;

    [Header("Jump Settings")]
    [SerializeField] private AnimationCurve jumpForceCurve;
    [SerializeField] private float maxJumpTime = 0.4f;
    [SerializeField] private float maxJumpForce = 20f;
    [SerializeField] private float jumpHeightMultiplier = 1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float dropPlatformDuration = 0.3f;
    public float MaxJumpTime => maxJumpTime;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 50f;
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float dashCooldown = 0.8f;
    [SerializeField] private LayerMask dashStop;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;
    public LayerMask DashStop => dashStop;

    [Header("Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformPassLayer;
    [SerializeField] private LayerMask ceilingLayer;
    [SerializeField] private float boxHeight = 0.1f;
    [SerializeField] private float boxLowAirHeight = 3f;
    public LayerMask GroundLayer => groundLayer;

    [Header("Attack Settings")]
    [SerializeField] private float attackBufferTime = 0.1f;
    [SerializeField] private float weaponChangeCooldown = 0.6f;

    [Header("Counter Settings")]
    [SerializeField] private float counterCooldown = 0.1f;
    [SerializeField] private Vector2 counterGroundOffset = new(1.2f, 0f);
    [SerializeField] private Vector2 counterAirOffset = new(1.2f, 0f);
    [SerializeField] private Vector2 counterBoxSize = new(1.6f, 1.2f);
    [SerializeField] private float counterHitDelay = 0.05f;
    [SerializeField] private LayerMask counterTargetLayer;
    private float lastWeaponChangeTime = -999f;
    public bool CanChangeWeapon => Time.time >= lastWeaponChangeTime + weaponChangeCooldown;
    public void MarkWeaponChanged() => lastWeaponChangeTime = Time.time;
    public bool AttackBuffered => attackBufferTimer > 0f;
    public bool CanUseCounter => Time.time >= lastCounterTime + counterCooldown;

    [Header("Energy Settings")]
    [SerializeField] private int maxEnergy = 100;
    [SerializeField] private int startEnergy = 100;
    [SerializeField] private GameObject energyOrbPrefab;
    [SerializeField] private int counterEnergyOrbCount = 20;
    [SerializeField] private Vector2 energyOrbSpawnRandomX = new(0.3f, 1.2f);
    [SerializeField] private Vector2 energyOrbSpawnRandomY = new(0.2f, 1f);
    public int MaxEnergy => maxEnergy;
    public int CounterEnergyOrbCount => counterEnergyOrbCount;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.1f;
    private bool isKnockback = false;
    private float knockbackTimer = 0f;

    [Header("Sound Settings")]
    public AudioClip Jump;
    public AudioClip Dash;
    public AudioClip Damage;
    public AudioClip Death;
    public AudioClip SpearNormal;
    public AudioClip SpearSkill;
    public AudioClip BowNormal;
    public AudioClip BowSkillCharging;
    public AudioClip BowSkillRelease;
    public AudioClip BombBoom;

    private Rigidbody2D rb;
    private BoxCollider2D boxCol;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    [HideInInspector] public bool isNoClip = false;
    [HideInInspector] public int facingDirection = 1;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isEdge;
    [HideInInspector] public bool isLowAir;
    [HideInInspector] public bool isGroundedLeft;
    [HideInInspector] public bool isGroundedCenter;
    [HideInInspector] public bool isGroundedRight;

    [HideInInspector] public bool isJumping;
    [HideInInspector] public float jumpTimeCounter;
    [HideInInspector] public float jumpBufferTimer;
    [HideInInspector] public float coyoteTimer;

    [HideInInspector] public bool canAirDash = true;
    [HideInInspector] public float lastDashTime = -999f;
    [HideInInspector] public float dashTimer;

    [HideInInspector] public float attackBufferTimer = 0f;
    [HideInInspector] public float normalGravityScale;
    [HideInInspector] public bool isTouchingCeiling;

    public static WeaponType CurrentWeapon = WeaponType.Spear;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private PlayerStateMachine stateMachine;
    private bool prevSkillAvailable = false;
    private bool isDroppingPlatform = false;
    private bool wasDownKeyHeldLastFrame = false;
    private float lastCounterTime = -999f;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        AttackController = GetComponent<AttackController>();
        originalColliderSize = boxCol.size;
        originalColliderOffset = boxCol.offset;
        normalGravityScale = rb.gravityScale;
        CurrentEnergy = Mathf.Clamp(startEnergy, 0, maxEnergy);

        if (Instance != null && Instance != this)
        {
            Debug.Log("Player Destroyed");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Start()
    {
        SetPlayer();
        AttackController.Initialize(CurrentWeapon);
        stateMachine = new PlayerStateMachine();
        stateMachine.Initialize(new LocomotionState(this, stateMachine));
        SetEffectState(PlayerEffectState.None);
        NotifyWeaponChanged(AttackController.CurrentWeapon);
        NotifyEnergyChanged();
    }

    public void Update()
    {
        UpdateGrounded();
        UpdateCeiling();
        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;
        if (attackBufferTimer > 0f)
            attackBufferTimer -= Time.deltaTime;

        bool nowAvailable = AttackController.CanUseSkill;
        if (!prevSkillAvailable && nowAvailable)
            Debug.Log("[Skill] 스킬 재사용 가능");
        prevSkillAvailable = nowAvailable;

        if (isGrounded && !wasDownKeyHeldLastFrame && DownHeld && !isDroppingPlatform)
            StartCoroutine(DropThroughPlatform());

        wasDownKeyHeldLastFrame = DownHeld;

        if (isKnockback)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f || isNoClip)
                isKnockback = false;
        }

        stateMachine.Update();
    }

    public void FixedUpdate() => stateMachine.FixedUpdate();

    public void SetPlayer()
    {
        InputManager.Instance.RegisterPlayer(this);
        InputManager.Instance.CurrentContext = InputManager.InputContext.Gameplay;
    }

    public void SetEffectState(PlayerEffectState newState)
    {
        if (CurrentEffectState == newState)
            return;

        CurrentEffectState = newState;
        OnEffectStateChanged?.Invoke(newState);
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }

    public bool HasEnoughEnergy(int amount) => CurrentEnergy >= amount;

    public bool TryConsumeEnergy(int amount)
    {
        if (amount <= 0)
            return true;

        if (CurrentEnergy < amount)
            return false;

        CurrentEnergy -= amount;
        NotifyEnergyChanged();
        return true;
    }

    public void RestoreEnergy(int amount)
    {
        if (amount <= 0)
            return;

        int nextEnergy = Mathf.Clamp(CurrentEnergy + amount, 0, maxEnergy);
        if (nextEnergy == CurrentEnergy)
            return;

        CurrentEnergy = nextEnergy;
        NotifyEnergyChanged();
    }

    public void SpawnCounterEnergyOrbs(Vector3 origin)
    {
        SpawnEnergyOrbs(counterEnergyOrbCount, origin);
    }

    public void SpawnEnergyOrbs(int amount, Vector3 origin)
    {
        if (amount <= 0)
            return;

        if (energyOrbPrefab == null)
        {
            Debug.LogWarning("[EnergyOrb] 에너지 오브 프리팹이 비어있습니다.");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPosition = origin + GetRandomEnergyOrbSpawnOffset();
            GameObject orbObject = Instantiate(energyOrbPrefab, spawnPosition, Quaternion.identity);
            if (!orbObject.TryGetComponent<EnergyOrb>(out EnergyOrb orb))
            {
                Debug.LogWarning("[EnergyOrb] EnergyOrb 컴포넌트가 프리팹에 없습니다.");
                Destroy(orbObject);
                continue;
            }

            orb.Initialize(this);
        }
    }

    private Vector3 GetRandomEnergyOrbSpawnOffset()
    {
        float xDistance = UnityEngine.Random.Range(energyOrbSpawnRandomX.x, energyOrbSpawnRandomX.y);
        float x = xDistance * (UnityEngine.Random.value < 0.5f ? -1f : 1f);
        float y = UnityEngine.Random.Range(energyOrbSpawnRandomY.x, energyOrbSpawnRandomY.y);
        return new Vector3(x, y, 0f);
    }

    private void NotifyEnergyChanged()
    {
        OnEnergyChanged?.Invoke(CurrentEnergy, maxEnergy);
    }

    private void UpdateGrounded()
    {
        bool wasGrounded = isGrounded;
        Vector2 bottom = (Vector2)boxCol.bounds.center + Vector2.down * boxCol.bounds.extents.y;
        Vector2 topAlignedY = bottom - Vector2.up * (boxHeight * 0.5f);
        float totalWidth = boxCol.bounds.size.x;
        float leftWidth = totalWidth * 0.3f;
        float centerWidth = totalWidth * 0.4f;
        float rightWidth = totalWidth * 0.3f;

        Vector2 leftCenter = topAlignedY + Vector2.left * (centerWidth * 0.5f + leftWidth * 0.5f);
        Vector2 centerCenter = topAlignedY;
        Vector2 rightCenter = topAlignedY + Vector2.right * (centerWidth * 0.5f + rightWidth * 0.5f);

        isGroundedLeft = Physics2D.BoxCast(leftCenter, new Vector2(leftWidth, boxHeight), 0f, Vector2.down, 0f, groundLayer).collider != null;
        isGroundedCenter = Physics2D.BoxCast(centerCenter, new Vector2(centerWidth, boxHeight), 0f, Vector2.down, 0f, groundLayer).collider != null;
        isGroundedRight = Physics2D.BoxCast(rightCenter, new Vector2(rightWidth, boxHeight), 0f, Vector2.down, 0f, groundLayer).collider != null;

        isGrounded = !isJumping && (isGroundedLeft || isGroundedCenter || isGroundedRight) && rb.linearVelocity.y == 0;
        isEdge = (isGroundedLeft || isGroundedRight) && !isGroundedCenter;

        Vector2 lowAirSize = new(totalWidth, boxLowAirHeight);
        Vector2 lowAirOrigin = bottom - Vector2.up * (boxLowAirHeight * 0.5f);
        isLowAir = Physics2D.BoxCast(lowAirOrigin, lowAirSize, 0f, Vector2.down, 0f, groundLayer).collider != null;

        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        if (!wasGrounded && isGrounded)
        {
            canAirDash = true;
            AttackController.ResetCombo();
            AttackController.ResetAirborneCombo();
        }

        if (wasGrounded && !isGrounded)
            AttackController.ResetCombo();
    }

    private void UpdateCeiling()
    {
        Vector2 boxSize = new(boxCol.bounds.size.x, boxHeight);
        Vector2 origin = (Vector2)boxCol.bounds.center + Vector2.up * boxCol.bounds.extents.y;
        isTouchingCeiling = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.up, 0f, ceilingLayer).collider != null;
    }

#if UNITY_EDITOR
    [ContextMenu("박스캐스트 시각화")]
    private void DebugDrawBoxCastGizmos()
    {
        if (boxCol == null)
            boxCol = GetComponent<BoxCollider2D>();

        Vector2 bottom = (Vector2)boxCol.bounds.center + Vector2.down * boxCol.bounds.extents.y;
        Vector2 topAlignedY = bottom - Vector2.up * (boxHeight * 0.5f);
        float totalWidth = boxCol.bounds.size.x;
        float leftWidth = totalWidth * 0.3f;
        float centerWidth = totalWidth * 0.4f;
        float rightWidth = totalWidth * 0.3f;

        Vector2 leftCenter = topAlignedY + Vector2.left * (centerWidth * 0.5f + leftWidth * 0.5f);
        Vector2 centerCenter = topAlignedY;
        Vector2 rightCenter = topAlignedY + Vector2.right * (centerWidth * 0.5f + rightWidth * 0.5f);

        DrawDebugBox(leftCenter, leftWidth, boxHeight, Color.yellow);
        DrawDebugBox(centerCenter, centerWidth, boxHeight, Color.white);
        DrawDebugBox(rightCenter, rightWidth, boxHeight, Color.yellow);

        Vector2 lowAirCenter = bottom - Vector2.up * (boxLowAirHeight * 0.5f);
        DrawDebugBox(lowAirCenter, totalWidth, boxLowAirHeight, Color.cyan);

        Vector2 ceilingCenter = (Vector2)boxCol.bounds.center + Vector2.up * (boxCol.bounds.extents.y + boxHeight * 0.5f);
        DrawDebugBox(ceilingCenter, totalWidth, boxHeight, Color.red);
    }

    private void DrawDebugBox(Vector2 center, float width, float height, Color color)
    {
        Vector2 half = new Vector2(width, height) * 0.5f;
        Debug.DrawLine(center - half, center + new Vector2(half.x, -half.y), color, 1f);
        Debug.DrawLine(center + new Vector2(half.x, -half.y), center + half, color, 1f);
        Debug.DrawLine(center + half, center + new Vector2(-half.x, half.y), color, 1f);
        Debug.DrawLine(center + new Vector2(-half.x, half.y), center - half, color, 1f);
    }
#endif

    public void HandleMove(float speed)
    {
        if (isKnockback)
            return;

        rb.linearVelocity = new Vector2(MoveInput * speed, rb.linearVelocity.y);

        if (MoveInput != 0)
        {
            facingDirection = MoveInput > 0 ? 1 : -1;
            transform.rotation = Quaternion.Euler(0f, facingDirection == -1 ? 180f : 0f, 0f);
        }
    }

    public void HandleJump()
    {
        if (isJumping && CurrentStateType == PlayerStateType.Locomotion)
        {
            jumpTimeCounter += Time.fixedDeltaTime;
            float t = jumpTimeCounter / maxJumpTime;
            float force = jumpForceCurve.Evaluate(t) * maxJumpForce * jumpHeightMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        }
    }

    public void HandleFastFall()
    {
        if (!isGrounded && !isJumping && DownHeld && rb.linearVelocity.y < 0 && CurrentStateType == PlayerStateType.Locomotion)
        {
            isJumping = false;
            Animator.Play("Fall");
            SetEffectState(PlayerEffectState.FastFall);
            rb.gravityScale = fastFallGravityScale;
        }
        else
        {
            SetEffectState(PlayerEffectState.None);
            rb.gravityScale = normalGravityScale;
        }
    }

    public void StopRising()
    {
        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.3f);
        jumpTimeCounter = maxJumpTime;
    }

    public void Die()
    {
        stateMachine.ChangeState(new DieState(this, stateMachine));
    }

    private IEnumerator DropThroughPlatform()
    {
        isDroppingPlatform = true;

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCol.size, 0f, platformPassLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<PlatformEffector2D>(out _))
                Physics2D.IgnoreCollision(boxCol, hit, true);
        }

        yield return new WaitForSeconds(dropPlatformDuration);

        foreach (Collider2D hit in hits)
        {
            if (hit != null)
                Physics2D.IgnoreCollision(boxCol, hit, false);
        }

        isDroppingPlatform = false;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (!CanTakeDamage)
            return;

        isKnockback = true;
        knockbackTimer = knockbackDuration;

        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    public void MarkCounterUsed() => lastCounterTime = Time.time;
    public Vector2 GetCounterOffset(bool groundedVariant) => groundedVariant ? counterGroundOffset : counterAirOffset;
    public void NotifyWeaponChanged(WeaponType weapon) => OnChangeWeapon?.Invoke(weapon);
    public void NotifyChainAttackFinished() => OnChainAttackFinished?.Invoke();
    public void NotifyCounterTry(bool hasHitMonster) => OnCounterTry?.Invoke(hasHitMonster);
    public void ConsumeAttackBuffer() => attackBufferTimer = 0f;
    public Rigidbody2D Rigidbody => rb;
    public BoxCollider2D BoxCollider => boxCol;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Vector2 OriginalColliderSize => originalColliderSize;
    public Vector2 OriginalColliderOffset => originalColliderOffset;
    public Vector2 CounterBoxSize => counterBoxSize;
    public float CounterHitDelay => counterHitDelay;
    public LayerMask CounterTargetLayer => counterTargetLayer.value == 0 ? LayerMask.GetMask("Enemy") : counterTargetLayer;
    public bool IsKnockbackActive => isKnockback;
}