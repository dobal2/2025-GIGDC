using UnityEngine;
using static InputManager;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // 입력값 세터
    public float MoveInput { private get; set; }
    public bool JumpPressed { private get; set; }
    public bool JumpHeld { private get; set; }
    public bool DashPressed { private get; set; }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float maxJumpTime = 0.3f;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isJumping = false;
    private float jumpTimeCounter;

    private bool canAirDash = true;
    private float lastDashTime = -999f;
    private int facingDirection = 1;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        InputManager.Instance.RegisterPlayer(this);
        InputManager.Instance.currentContext = InputContext.Gameplay;
    }

    void Update()
    {
        UpdateGrounded();
    }

    void FixedUpdate()
    {
        HandleMove();
        HandleJump();
        HandleDash();
    }

    void UpdateGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded) canAirDash = true;
    }

    void HandleMove()
    {
        rb.linearVelocity = new Vector2(MoveInput * moveSpeed, rb.linearVelocity.y);

        if (MoveInput != 0)
            facingDirection = MoveInput > 0 ? 1 : -1;
    }

    void HandleJump()
    {
        if (JumpPressed && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (JumpHeld && isJumping)
        {
            if (jumpTimeCounter > 0)
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

    void HandleDash()
    {
        if (!DashPressed) return;
        if (Time.time < lastDashTime + dashCooldown) return;
        if (!isGrounded && !canAirDash) return;

        Vector2 dashDir = new(facingDirection, 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDir, dashDistance, groundLayer);
        float actualDashDist = hit.collider ? hit.distance : dashDistance;

        rb.MovePosition(rb.position + dashDir * actualDashDist);

        lastDashTime = Time.time;
        if (!isGrounded) canAirDash = false;
    }
}