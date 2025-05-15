using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public float MoveInput { private get; set; }
    public bool JumpHeld { private get; set; }
    public bool DashPressed { private get; set; }

    private bool jumpBuffered = false;
    public bool JumpPressed
    {
        set { if (value) jumpBuffered = true; }
    }

    private enum PlayerState { Normal, Crouching, Dashing }
    private PlayerState currentState = PlayerState.Normal;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float maxJumpTime = 0.3f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchColliderHeightMultiplier = 0.5f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCol;

    private bool isGrounded;
    private bool isJumping;
    private float jumpTimeCounter;
    private bool canAirDash = true;
    private float lastDashTime = -999f;
    private float dashTimer;
    private int facingDirection = 1;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        originalColliderSize = boxCol.size;
        originalColliderOffset = boxCol.offset;
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
    }

    void FixedUpdate()
    {
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
    }

    void UpdateGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded) canAirDash = true;
    }

    void HandleStateTransitions()
    {
        // Crouch
        if (Input.GetKey(InputManager.Instance.keyData.Player.DownMoveKey) && isGrounded && currentState != PlayerState.Dashing)
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

        // Dash
        if (DashPressed && Time.time >= lastDashTime + dashCooldown)
        {
            if (isGrounded || canAirDash)
            {
                currentState = PlayerState.Dashing;
                dashTimer = dashDuration;
                lastDashTime = Time.time;
                if (!isGrounded) canAirDash = false;
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
        if (jumpBuffered && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBuffered = false;
        }

        if (JumpHeld && isJumping)
        {
            if (jumpTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTimeCounter -= Time.fixedDeltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (!JumpHeld)
        {
            isJumping = false;
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
}