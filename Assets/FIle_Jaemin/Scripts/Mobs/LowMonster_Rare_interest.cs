using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class LowMonster_Rare_interest : Monster
{
    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float playerNoticeDistance = 5f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float skilledSpeed = 10f;
    [SerializeField] private GameObject dashEffectPrefab;
    [SerializeField] private Transform effectPos;
    [SerializeField] private Transform attackTransform;
    [SerializeField] private Vector2 attackSize;
    [SerializeField] private ParticleSystem inkTrail;
    
    private Coroutine dashCoroutine;


    private int nextMove = 1;
    private bool isDashing;
    private bool canEffect = true;
    private bool canFlip = true;
    private bool isGrounded;

    protected override void Start()
    {
        base.Start();
        rigid = GetComponent<Rigidbody2D>();
    }

    protected override void Attack()
    {
        if (dashCoroutine != null || isStunned) return;
        dashCoroutine = StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        if (isStunned)
        {
            dashCoroutine = null;
            yield break;
        }
        canFlip = false;
        yield return new WaitForSeconds(1f);
        
        anim.SetBool("Dashing", true);
        isDashing = true;
        canFlip = false;

        float dashDirection = facingRight ? 1f : -1f;
        rigid.linearVelocity = new Vector2(skilledSpeed * dashDirection, rigid.linearVelocity.y);

        yield return new WaitForSeconds(0.5f); // 대시 유지 시간

        rigid.linearVelocity = new Vector2(0, rigid.linearVelocity.y);
        anim.SetBool("Dashing", false);

        isDashing = false;

        StartCoroutine(WaitToAttack(attackCoolDown));

        yield return new WaitForSeconds(0.5f);
        canFlip = true;
        dashCoroutine = null;
    }


    private void FixedUpdate()
    {
        // 바닥 체크
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);
        float distanceY = player.position.y - transform.position.y;
        float absDistanceY = Mathf.Abs(distanceY);

        // 플레이어 감지 및 방향 전환
        if (absDistanceX <= playerNoticeDistance && absDistanceY < 2f)
        {
            FacePlayer();

            if (canAttack && !isStunned)
            {
                Attack();
            }

        }

        if (!isDashing && canFlip && isGrounded && absDistanceX > stopDistance && !isStunned)
        {
            Move();
        }
        else
        {
            anim.SetBool("Walking", false);
        }

        // 대시 중 히트박스 체크
        if (isDashing)
        {
            CheckDashHitbox();
        }
    }


    protected void Update()
    {
        Effect();
    }

    private void FacePlayer()
    {
        if (!isDashing && canFlip && isGrounded)
        {
            bool shouldFaceRight = player.position.x > transform.position.x;
            if (shouldFaceRight != facingRight)
            {
                Flip();
                nextMove = facingRight ? 1 : -1;
            }
        }
    }

    private void CheckDashHitbox()
    {
        Collider2D[] collidersEnemies = Physics2D.OverlapBoxAll(attackTransform.position, attackSize, 0);
        foreach (var collider in collidersEnemies)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
    }

    private void Move()
    {
        if (!canFlip) return;

        anim.SetBool("Walking", true);
        rigid.linearVelocity = new Vector2(speed * nextMove, rigid.linearVelocity.y);
        
        if (!inkTrail.isPlaying && !inkTrail.main.loop)
        {
            inkTrail.Play();
        }

        DetectGroundAndWalls();
    }

    public override void TakeDamage(float amount)
    {
        // Dash 상태일 경우 중단
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
        }

        isDashing = false;
        anim.SetBool("Dashing", false);
        rigid.linearVelocity = Vector2.zero;
        canFlip = true;
        
        hp -= amount;
        isStunned = true;
        
        GameObject newInkExplosion = Instantiate(inkHitEffect, transform.position, Quaternion.identity);
        Destroy(newInkExplosion,2);

        // 공격 중이라면 끊기
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;

            anim.ResetTrigger("Attack");
            anim.Play("Idle");
        }

        TakeDamageAnimation();

        if (hp <= 0) Die();
    }

    private void DetectGroundAndWalls()
    {
        Vector2 groundCheckPos = rigid.position + new Vector2(nextMove, 1f);
        Vector2 wallCheckPos = rigid.position + new Vector2(nextMove * 0.5f, 1f);

        Debug.DrawRay(groundCheckPos, Vector2.down * 1.5f, Color.green);
        Debug.DrawRay(wallCheckPos, 1.0f * nextMove * Vector2.right, Color.blue);

        bool noGround = !Physics2D.Raycast(groundCheckPos, Vector2.down, 1.5f, groundLayer);
        bool hitWall = Physics2D.Raycast(wallCheckPos, Vector2.right * nextMove, 1.0f, wallLayer);

        if ((noGround || hitWall) && !isDashing && isGrounded)
        {
            nextMove *= -1;
            if (canFlip)
            {
                Flip();
            }
        }
    }

    private void Effect()
    {
        if (isDashing && canEffect)
        {
            GameObject effect = Instantiate(dashEffectPrefab, effectPos.position, Quaternion.identity);

            if (!facingRight)
                effect.GetComponent<InterestEffect>().Flip();

            canEffect = false;
            StartCoroutine(ResetCanEffect());
        }
    }

    private IEnumerator ResetCanEffect()
    {
        yield return new WaitForSeconds(0.1f);
        canEffect = true;
    }

    protected override void Die()
    {
        base.Die();
    }

    // public override void TakeDamage(float amount, Vector2 knockBackDir)
    // {
    //     base.TakeDamage(amount, knockBackDir);
    // }

    private void OnDrawGizmosSelected()
    {
        if (attackTransform != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawCube(attackTransform.position, attackSize);
        }
    }
}
