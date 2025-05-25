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

    private int nextMove = 1;
    private bool isDashing;
    private bool canEffect = true;
    private bool canFlip = true;

    protected override void Start()
    {
        base.Start();
        rigid = GetComponent<Rigidbody2D>();
    }

    protected override void Attack()
    {
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        anim.SetBool("Dashing", true);
        isDashing = true;
        canFlip = false;

        float dashDirection = facingRight ? 1f : -1f;
        rigid.linearVelocity = new Vector2(skilledSpeed * dashDirection, rigid.linearVelocity.y);

        yield return new WaitForSeconds(0.5f);

        rigid.linearVelocity = Vector2.zero;
        anim.SetBool("Dashing", false);
        StartCoroutine(WaitToAttack(attackCoolDown));

        isDashing = false;

        yield return new WaitForSeconds(0.5f);
        canFlip = true;
    }

    private void FixedUpdate()
    {
        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);
        float distanceY = player.position.y - transform.position.y;
        float absDistanceY = Mathf.Abs(distanceY);

        // 플레이어 감지 및 방향 전환
        if (absDistanceX <= playerNoticeDistance && absDistanceY < 2f)
        {
            FacePlayer();

            if (canAttack)
                Attack();
        }

        // 이동
        if (!isDashing && canFlip && absDistanceX > stopDistance)
        {
            Move();
        }
        else
        {
            anim.SetBool("Walking", false);
        }
        
        if (isDashing)
        {
            CheckDashHitbox();
        }
    }

    private void Update()
    {
        Effect();
    }
    
    private void FacePlayer()
    {
        if (!isDashing && canFlip)
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
        // 방향에 따라 오프셋 방향 결정
    
        // OverlapBox로 범위 감지
        Collider2D[] collidersEnemies = Physics2D.OverlapBoxAll(attackTransform.position,attackSize,0);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.tag == "Player")
            {
                collidersEnemies[i].GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
    }

    
    private void Move()
    {
        if (!canFlip) return;

        anim.SetBool("Walking", true);
        rigid.linearVelocity = new Vector2(speed * nextMove, rigid.linearVelocity.y);

        DetectGroundAndWalls();
    }

    /// <summary>
    /// 낭떠러지와 벽 감지 후 방향 반전
    /// </summary>
    private void DetectGroundAndWalls()
    {
        Vector2 groundCheckPos = rigid.position + new Vector2(nextMove, 1f);
        Vector2 wallCheckPos = rigid.position + new Vector2(nextMove * 0.5f, 1f);

        Debug.DrawRay(groundCheckPos, Vector2.down * 1.5f, Color.green);
        Debug.DrawRay(wallCheckPos, Vector2.right * nextMove * 1.0f, Color.blue);

        bool noGround = !Physics2D.Raycast(groundCheckPos, Vector2.down, 1.5f, groundLayer);
        bool hitWall = Physics2D.Raycast(wallCheckPos, Vector2.right * nextMove, 1.0f, wallLayer);

        if ((noGround || hitWall) && !isDashing)
        {
            nextMove *= -1;
            if (canFlip)
            {
                Flip();
            }
        }
    }

    /// <summary>
    /// 대시 중 이펙트 생성
    /// </summary>
    private void Effect()
    {
        if (isDashing && canEffect)
        {
            GameObject effect = Instantiate(dashEffectPrefab, effectPos.position, Quaternion.identity);

            if (!facingRight)
                effect.GetComponent<InterestEffect>()?.Flip();

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
        gameObject.SetActive(false);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackTransform != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawCube(attackTransform.position,attackSize);
        }
        
    }
}
