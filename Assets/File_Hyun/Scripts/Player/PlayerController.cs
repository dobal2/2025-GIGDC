using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public float MoveInput { private get; set; }
    public bool DashPressed { private get; set; }
    public bool CrouchHeld { private get; set; }

    public bool JumpPressed
    {
        set { if (value) jumpBufferTimer = jumpBufferTime; }
    }

    private enum PlayerState { Normal, Crouching, Dashing }
    private PlayerState currentState = PlayerState.Normal;
    private PlayerState previousState = PlayerState.Normal;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float fastFallGravityScale = 6f;

    [Header("Jump Settings")]
    [SerializeField] private AnimationCurve jumpForceCurve;
    [SerializeField] private float maxJumpTime = 0.4f;
    [SerializeField] private float maxJumpForce = 20f;
    [SerializeField] private float jumpHeightMultiplier = 1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.6f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchColliderHeightMultiplier = 0.5f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCol;

    private bool isGrounded;
    private bool isJumping;
    private float jumpTimeCounter;
    private float jumpBufferTimer;
    private bool canAirDash = true;
    private float lastDashTime = -999f;
    private float dashTimer;
    private int facingDirection = 1;
    private float normalGravityScale;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        originalColliderSize = boxCol.size;
        originalColliderOffset = boxCol.offset;
        normalGravityScale = rb.gravityScale;
    }

    void Start()
    {
        InputManager.Instance.RegisterPlayer(this);
        InputManager.Instance.currentContext = InputManager.InputContext.Gameplay;
    }

    void Update()
    {
        UpdateGrounded();
        HandleStateTransitions();

        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (currentState != previousState)
        {
            ApplyConstraintsForState(currentState);
            previousState = currentState;
        }

        switch (currentState)
        {
            case PlayerState.Normal:
                HandleMove(moveSpeed);
                HandleJump();
                break;

            case PlayerState.Crouching:
                HandleMove(moveSpeed * crouchSpeedMultiplier);
                break;

            case PlayerState.Dashing:
                HandleDashMovement();
                break;
        }
        HandleFastFall();
    }

    void UpdateGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded) canAirDash = true;
    }

    void HandleStateTransitions()
    {
        if (CrouchHeld && isGrounded && currentState != PlayerState.Dashing)
        {
            if (currentState != PlayerState.Crouching)
            {
                currentState = PlayerState.Crouching;
                Vector2 newSize = new(originalColliderSize.x, originalColliderSize.y * crouchColliderHeightMultiplier);
                Vector2 newOffset = new(originalColliderOffset.x, originalColliderOffset.y + (originalColliderSize.y * (crouchColliderHeightMultiplier - 1f) / 2f));
                boxCol.size = newSize;
                boxCol.offset = newOffset;
            }
            return;
        }
        else if (currentState == PlayerState.Crouching)
        {
            currentState = PlayerState.Normal;
            boxCol.size = originalColliderSize;
            boxCol.offset = originalColliderOffset;
        }

        if (DashPressed && Time.time >= lastDashTime + dashCooldown)
        {
            if (isGrounded || canAirDash)
            {
                currentState = PlayerState.Dashing;
                dashTimer = dashDuration;
                lastDashTime = Time.time;
                if (!isGrounded) canAirDash = false;

                jumpBufferTimer = 0f;
                isJumping = false;
            }
        }
    }

    void HandleMove(float speed)
    {
        rb.linearVelocity = new Vector2(MoveInput * speed, rb.linearVelocity.y);
        if (MoveInput != 0)
            facingDirection = MoveInput > 0 ? 1 : -1;
    }

    void HandleJump()
    {
        if (jumpBufferTimer > 0f && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = 0f;
            jumpBufferTimer = 0f;
        }

        if (isJumping)
        {
            if (jumpTimeCounter >= maxJumpTime)
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

    void HandleDashMovement()
    {
        Vector2 dashDir = new(facingDirection, 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDir, 0.5f, wallLayer);
        if (hit.collider)
        {
            currentState = PlayerState.Normal;
            return;
        }

        rb.linearVelocity = new Vector2(dashSpeed * facingDirection, 0);
        dashTimer -= Time.fixedDeltaTime;
        if (dashTimer <= 0f)
        {
            currentState = PlayerState.Normal;
        }
    }

    void HandleFastFall()
    {
        if (!isGrounded && !isJumping && currentState != PlayerState.Dashing && CrouchHeld)
            rb.gravityScale = fastFallGravityScale;
        else
            rb.gravityScale = normalGravityScale;
    }

    public void StopRising()
    {
        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.2f);

        isJumping = false;
        jumpTimeCounter = maxJumpTime;
    }

    void ApplyConstraintsForState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Dashing:
                rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                break;
            default:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                break;
        }
    }
}