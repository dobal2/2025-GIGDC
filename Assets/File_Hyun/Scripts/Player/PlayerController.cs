using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public float MoveInput { get; set; }
    public bool CrouchHeld { get; set; }
    public bool JumpHeld { get; set; }

    public bool JumpPressed
    {
        set { if (value) jumpBufferTimer = jumpBufferTime; }
    }

    public bool DashPressed
    {
        set { if (value) dashBufferTimer = dashBufferTime; }
    }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float fastFallGravityScale = 16f;
    public float MoveSpeed => moveSpeed;
    public float FastFallGravityScale => fastFallGravityScale;

    [Header("Jump Settings")]
    [SerializeField] private AnimationCurve jumpForceCurve;
    [SerializeField] private float maxJumpTime = 0.4f;
    [SerializeField] private float maxJumpForce = 20f;
    [SerializeField] private float jumpHeightMultiplier = 1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.2f;
    public AnimationCurve JumpForceCurve => jumpForceCurve;
    public float MaxJumpTime => maxJumpTime;
    public float MaxJumpForce => maxJumpForce;
    public float JumpHeightMultiplier => jumpHeightMultiplier;
    public float JumpBufferTime => jumpBufferTime;
    public float CoyoteTime => coyoteTime;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 50f;
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float dashCooldown = 0.8f;
    [SerializeField] private float dashBufferTime = 0.1f;
    [SerializeField] private LayerMask dashStop;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;
    public LayerMask DashStop => dashStop;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ceiling Check")]
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private float ceilingCheckRadius = 0.1f;
    [SerializeField] private LayerMask ceilingLayer;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchColliderHeightMultiplier = 0.5f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    public float CrouchColliderHeightMultiplier => crouchColliderHeightMultiplier;
    public float CrouchSpeedMultiplier => crouchSpeedMultiplier;

    private Rigidbody2D rb;
    private BoxCollider2D boxCol;

    [HideInInspector] public bool isGrounded;
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
    [HideInInspector] public float dashBufferTimer;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private PlayerStateMachine stateMachine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
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

        stateMachine = new PlayerStateMachine();
        stateMachine.Initialize(new PlayerIdleState(this, stateMachine));
    }

    void Update()
    {
        UpdateGrounded();
        UpdateCeiling();

        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;

        if (dashBufferTimer > 0f)
            dashBufferTimer -= Time.deltaTime;

        stateMachine.Update();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    void UpdateGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        if (!wasGrounded && isGrounded)
            canAirDash = true;
    }

    void UpdateCeiling()
    {
        isTouchingCeiling = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, ceilingLayer);
    }

    public void HandleMove(float speed)
    {
        rb.linearVelocity = new Vector2(MoveInput * speed, rb.linearVelocity.y);
        if (MoveInput != 0)
            facingDirection = MoveInput > 0 ? 1 : -1;
    }

    public void HandleJump()
    {
        if (isJumping)
        {
            if (!JumpHeld || jumpTimeCounter >= maxJumpTime || isTouchingCeiling)
            {
                isJumping = false;
                return;
            }

            jumpTimeCounter += Time.fixedDeltaTime;
            float t = jumpTimeCounter / maxJumpTime;
            float force = jumpForceCurve.Evaluate(t) * maxJumpForce * jumpHeightMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        }
    }

    public void HandleFastFall()
    {
        if (!isGrounded && !isJumping && CrouchHeld && rb.linearVelocity.y < 0)
            rb.gravityScale = fastFallGravityScale;
        else
            rb.gravityScale = normalGravityScale;
    }

    public void StopRising()
    {
        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.3f);

        isJumping = false;
        jumpTimeCounter = maxJumpTime;
    }

    public Rigidbody2D Rigidbody => rb;
    public BoxCollider2D BoxCollider => boxCol;
    public Vector2 OriginalColliderSize => originalColliderSize;
    public Vector2 OriginalColliderOffset => originalColliderOffset;
}