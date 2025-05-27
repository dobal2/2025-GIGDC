using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public AttackController AttackController { get; private set; }
    public Animator Animator { get; private set; }

    public enum PlayerEffectState
    {
        None, // 이펙트 없음
        GroundWalkDust, // 먼지
        Dash, // 대시 중
        FastFall, // 빠른 낙하
        SpearAirSkill, // 창 내려찍기
        BowSkillCharging, // 활 스킬 차징중
        BowSkillRelease, // 활 스킬 발사
        // 필요 시 추가
    }
    public event Action<PlayerEffectState> OnEffectStateChanged;

    public PlayerEffectState CurrentEffectState { get; private set; }
    public float MoveInput { get; set; }
    public bool DownHeld { get; set; }
    public bool JumpHeld { get; set; }
    public bool DashPressed { get; set; }
    public bool SkillPressed { get; set; }

    public bool JumpPressed
    {
        set { if (value) jumpBufferTimer = jumpBufferTime; }
    }

    public bool AttackPressed
    {
        set { if (value) attackBufferTimer = attackBufferTime; }
    }

    public PlayerStateType CurrentStateType => stateMachine.CurrentStateType;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float fastFallGravityScale = 18f;
    public float MoveSpeed => moveSpeed;
    public Vector2 CurrentVelocity => rb.linearVelocity;

    [Header("Jump Settings")]
    [SerializeField] private AnimationCurve jumpForceCurve;
    [SerializeField] private float maxJumpTime = 0.4f;
    [SerializeField] private float maxJumpForce = 20f;
    [SerializeField] private float jumpHeightMultiplier = 1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;
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
    [SerializeField] private LayerMask ceilingLayer;
    [SerializeField] private float boxWidth = 0.99f;
    [SerializeField] private float boxHeight = 0.1f;
    [SerializeField] private float boxLowAirHeight = 3f;
    public LayerMask GroundLayer => groundLayer;

    [Header("Attack Settings")]
    [SerializeField] private float attackBufferTime = 0.1f;
    [HideInInspector] public float attackBufferTimer = 0f;
    public bool AttackBuffered => attackBufferTimer > 0f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCol;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isLowAir;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public float jumpTimeCounter;
    [HideInInspector] public float jumpBufferTimer;
    [HideInInspector] public float coyoteTimer;
    [HideInInspector] public bool canAirDash = true;
    [HideInInspector] public float lastDashTime = -999f;
    [HideInInspector] public float dashTimer;
    [HideInInspector] public int facingDirection = 1;
    [HideInInspector] public float normalGravityScale;
    [HideInInspector] public bool isTouchingCeiling;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private PlayerStateMachine stateMachine;

    private bool prevSkillAvailable = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        Animator = GetComponent<Animator>();
        AttackController = GetComponent<AttackController>();
        originalColliderSize = boxCol.size;
        originalColliderOffset = boxCol.offset;
        normalGravityScale = rb.gravityScale;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        InputManager.Instance.RegisterPlayer(this);
        InputManager.Instance.currentContext = InputManager.InputContext.Gameplay;

        AttackController.Initialize(WeaponType.Bow);
        stateMachine = new PlayerStateMachine();
        stateMachine.Initialize(new PlayerLocomotionState(this, stateMachine));
        SetEffectState(PlayerEffectState.None);
    }

    void Update()
    {
        UpdateGrounded();
        UpdateCeiling();

        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;

        if (attackBufferTimer > 0f)
            attackBufferTimer -= Time.deltaTime;

        bool nowAvailable = AttackController.CanUseSkill;
        if (!prevSkillAvailable && nowAvailable)
        {
            Debug.Log("[Skill] 스킬 쿨타임 완료 - 사용 가능");
        }
        prevSkillAvailable = nowAvailable;

        stateMachine.Update();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void SetEffectState(PlayerEffectState newState)
    {
        if (CurrentEffectState == newState) return;
        CurrentEffectState = newState;
        OnEffectStateChanged?.Invoke(newState);
    }

    void UpdateGrounded()
    {
        bool wasGrounded = isGrounded;

        Vector2 boxSize = new(boxCol.bounds.size.x * boxWidth, boxHeight);
        Vector2 origin = (Vector2)boxCol.bounds.center + Vector2.down * boxCol.bounds.extents.y;
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, 0f, groundLayer);

        Vector2 lowAirSize = new(boxCol.bounds.size.x * boxWidth, boxLowAirHeight);
        RaycastHit2D lowAirHit = Physics2D.BoxCast(origin + Vector2.down * 1f, lowAirSize, 0f, Vector2.down, 0f, groundLayer);
        isLowAir = lowAirHit.collider != null;

        isGrounded = !isJumping && hit.collider != null;

        if (isGrounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        if (!wasGrounded && isGrounded)
        {
            canAirDash = true;
            AttackController.ResetCombo();
            AttackController.ResetAirborneCombo();
        }

        if (wasGrounded && !isGrounded)
        {
            AttackController.ResetCombo();
        }
    }

    void UpdateCeiling()
    {
        Vector2 boxSize = new(boxCol.bounds.size.x * boxWidth, boxHeight);
        Vector2 origin = (Vector2)boxCol.bounds.center + Vector2.up * boxCol.bounds.extents.y;
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.up, 0f, ceilingLayer);

        isTouchingCeiling = hit.collider != null;
    }

#if UNITY_EDITOR
    [ContextMenu("박스캐스트 시각화")]
    private void DebugDrawBoxCastGizmos()
    {
        if (boxCol == null)
            boxCol = GetComponent<BoxCollider2D>();

        Vector2 origin = (Vector2)boxCol.bounds.center + Vector2.down * boxCol.bounds.extents.y;

        DrawDebugBox(origin, boxCol.bounds.size.x * boxWidth, boxHeight, Color.green); // Ground
        DrawDebugBox(origin + Vector2.down * 1f, boxCol.bounds.size.x * boxWidth, boxLowAirHeight, Color.cyan); // LowAir
        DrawDebugBox((Vector2)boxCol.bounds.center + 0.5f * boxCol.bounds.size.y * Vector2.up, boxCol.bounds.size.x * boxWidth, boxHeight, Color.red); // Ceiling
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

    public void ConsumeAttackBuffer()
    {
        attackBufferTimer = 0f;
    }

    public Rigidbody2D Rigidbody => rb;
    public BoxCollider2D BoxCollider => boxCol;
    public Vector2 OriginalColliderSize => originalColliderSize;
    public Vector2 OriginalColliderOffset => originalColliderOffset;
}