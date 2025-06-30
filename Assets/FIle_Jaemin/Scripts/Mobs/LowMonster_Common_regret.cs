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
            DetectGroundAndWalls();
        }
    }

    private void DetectGroundAndWalls()
    {
        Vector2 groundCheckPos = rigid.position + new Vector2(nextMove, 1f);
        Vector2 wallCheckPos = rigid.position + new Vector2(nextMove * 0.5f, transform.localScale.y/2);

        Debug.DrawRay(groundCheckPos, Vector2.down * 1.5f, Color.green);
        Debug.DrawRay(wallCheckPos, Vector2.right * nextMove * 1.5f, Color.blue);

        bool noGround = !Physics2D.Raycast(groundCheckPos, Vector2.down, 1.5f, groundLayer);
        bool hitWall = Physics2D.Raycast(wallCheckPos, Vector2.right * nextMove, 1.5f, wallLayer);

        if ((noGround || hitWall) && isGrounded)
        {
            nextMove *= -1;
            Flip();
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
