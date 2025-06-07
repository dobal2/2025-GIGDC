using UnityEngine;

public class LowMonster_Common_regret : Monster
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackRadius;
    [SerializeField] private float stopDistance;

    private int nextMove;
    public bool canMove = true;
    private bool isGrounded;

    protected override void Start()
    {
        base.Start();
        SetRandomMoveDirection();
    }

    private void FixedUpdate()
    {
        // 바닥 체크
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        Move();

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= stopDistance)
        {
            canMove = false;

            if (canAttack)
            {
                // 땅에 있을 때만 방향 전환
                if (isGrounded)
                {
                    if (player.transform.position.x > transform.position.x && !facingRight)
                    {
                        Flip();
                        nextMove = 1;
                    }
                    else if (player.transform.position.x < transform.position.x && facingRight)
                    {
                        Flip();
                        nextMove = -1;
                    }
                }

                Attack();
            }
        }
        else
        {
            canMove = true;
        }

        // 이동 중 방향 보정 (공중에 있을 때는 제외)
        if (isGrounded)
        {
            if (nextMove == -1 && facingRight)
            {
                Flip();
            }
            else if (nextMove == 1 && !facingRight)
            {
                Flip();
            }
        }
    }

    private void SetRandomMoveDirection()
    {
        nextMove = Random.Range(-1, 2);
        if (nextMove == 0)
        {
            SetRandomMoveDirection();
        }
    }

    private void Update()
    {
        // nothing
    }

    private void Move()
    {
        if (canMove && isGrounded)
        {
            rigid.linearVelocity = new Vector2(speed * nextMove, rigid.linearVelocity.y);
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        if (isGrounded)
        {
            GroundDetector();
            WallDetector();
        }
    }

    private void GroundDetector()
    {
        Vector2 frontVec = new Vector2((rigid.position.x + nextMove), rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D groundRayHit = Physics2D.Raycast(frontVec, Vector3.down, 1f, groundLayer);
        if (groundRayHit.collider == null)
        {
            Debug.Log("NoGround Detected");
            nextMove *= -1;
        }
    }

    private void WallDetector()
    {
        Vector2 frontVec = rigid.position + new Vector2(nextMove, 0);
        Debug.DrawRay(frontVec, new Vector2(nextMove, 0), Color.blue);
        RaycastHit2D wallRayHit = Physics2D.Raycast(frontVec, new Vector2(nextMove, 0), 1f, wallLayer);

        if (wallRayHit.collider != null)
        {
            Debug.Log("Wall Detected");
            nextMove *= -1;
        }
    }

    protected override void Attack()
    {
        rigid.linearVelocity = Vector2.zero;
        anim.SetTrigger("Attack");
        StartCoroutine(WaitToAttack(attackCoolDown));
    }

    public void AttackOverlapCircle()
    {
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackRadius);
        foreach (var target in collidersEnemies)
        {
            if (target.CompareTag("Player"))
            {
                target.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackCheck != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(attackCheck.position, attackRadius);
        }
    }
}
