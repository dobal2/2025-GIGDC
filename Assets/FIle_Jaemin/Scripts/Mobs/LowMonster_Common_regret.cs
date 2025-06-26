using System.Collections;
using UnityEngine;

public class LowMonster_Common_regret : Monster
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackRadius;
    [SerializeField] private float stopDistance;
    [SerializeField] private ParticleSystem inkTrail;


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
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        Move();

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= stopDistance)
        {
            canMove = false;

            if (canAttack && !isStunned)
            {
                if (isGrounded)
                {
                    if (player.position.x > transform.position.x && !facingRight)
                    {
                        Flip();
                        nextMove = 1;
                    }
                    else if (player.position.x < transform.position.x && facingRight)
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

        if (isGrounded)
        {
            if (nextMove == -1 && facingRight) Flip();
            else if (nextMove == 1 && !facingRight) Flip();
        }
    }

    private void SetRandomMoveDirection()
    {
        nextMove = Random.Range(-1, 2);
        if (nextMove == 0) SetRandomMoveDirection();
    }

    private void Move()
    {
        if (canMove && isGrounded && !isStunned)
        {
            rigid.linearVelocity = new Vector2(speed * nextMove, rigid.linearVelocity.y);
            anim.SetBool("isWalking", true);
            if (!inkTrail.isPlaying && !inkTrail.loop)
            {
                inkTrail.Play();
            }
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
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(frontVec, Vector2.down, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(frontVec, Vector2.down, 1f, groundLayer);
        if (hit.collider == null)
        {
            Debug.Log("NoGround Detected");
            nextMove *= -1;
        }
    }

    private void WallDetector()
    {
        Vector2 frontVec = rigid.position + new Vector2(nextMove, 0);
        Debug.DrawRay(frontVec, new Vector2(nextMove, 0), Color.blue);
        RaycastHit2D hit = Physics2D.Raycast(frontVec, new Vector2(nextMove, 0), 1f, wallLayer);
        if (hit.collider != null)
        {
            Debug.Log("Wall Detected");
            nextMove *= -1;
        }
    }

    protected override void Attack()
    {
        if (attackCoroutine != null || isStunned) return;

        rigid.linearVelocity = Vector2.zero;
        anim.SetTrigger("Attack");

        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(attackCoolDown);
        attackCoroutine = null;
    }

    public void AttackOverlapCircle()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                target.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    protected override void Die()
    {
        base.Die();
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
