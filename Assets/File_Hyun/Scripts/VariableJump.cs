using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CurveJump : MonoBehaviour
{
    [SerializeField] private AnimationCurve jumpForceCurve;
    [SerializeField] private float jumpDuration = 0.4f;
    [SerializeField] private float maxJumpForce = 20f;
    [SerializeField] private float fallMultiplier = 3f;
    [SerializeField] private float jumpHeightMultiplier = 1f;

    private Rigidbody2D rb;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private bool isGrounded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            jumpTimer = 0f;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;

            if (rb.linearVelocity.y > 0)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.2f);
        }
    }

    void FixedUpdate()
    {
        if (isJumping)
        {
            jumpTimer += Time.fixedDeltaTime;

            if (jumpTimer >= jumpDuration)
            {
                isJumping = false;
                return;
            }

            float t = jumpTimer / jumpDuration;
            float force = jumpForceCurve.Evaluate(t) * maxJumpForce * jumpHeightMultiplier;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += (fallMultiplier - 1f) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}