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

    private int nextMove;
    private bool isDashing;
    private bool canEffect = true;
    private bool canFlip = true;

    protected override void Start()
    {
        base.Start();
        rigid = GetComponent<Rigidbody2D>();
        SetRandomMoveDirection();
    }

    private void SetRandomMoveDirection()
    {
        do
        {
            nextMove = Random.Range(-1, 2);
        } while (nextMove == 0);
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

        // 감지 범위 안에 플레이어가 있는 경우
        if (absDistanceX <= playerNoticeDistance && absDistanceY < 2)
        {
            if (canFlip)
            {
                bool shouldFlip = (player.position.x > transform.position.x) != facingRight;
                if (shouldFlip)
                {
                    Flip();
                    nextMove = facingRight ? 1 : -1;
                }
            }

            if (canAttack)
            {
                Attack();
                //rigid.linearVelocity = Vector2.zero;
            }
        }
        
        if (absDistanceX > stopDistance)
        {
            Move();
        }
        else
        {
            anim.SetBool("Walking",false);
        }
    }


    private void Update()
    {
        if (isDashing && canEffect)
        {
            GameObject effect = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);

            if (!facingRight)
            {
                effect.GetComponent<InterestEffect>().Flip();
            }

            canEffect = false;
            StartCoroutine(ResetCanEffect());
        }
    }

    private IEnumerator ResetCanEffect()
    {
        yield return new WaitForSeconds(0.1f);
        canEffect = true;
    }

    private void Move()
    {
        if (canFlip)
        {
            anim.SetBool("Walking",true);
            rigid.linearVelocity = new Vector2(speed * nextMove, rigid.linearVelocity.y);
            GroundDetector();
            WallDetector();   
        }
        else
        {
            anim.SetBool("Walking",false);
        }
    }

    private void GroundDetector()
    {
        Vector2 checkPos = rigid.position + new Vector2(nextMove, 0);
        Debug.DrawRay(checkPos, Vector2.down * 1.5f, Color.green);

        if (!Physics2D.Raycast(checkPos, Vector2.down, 1.5f, groundLayer))
        {
            Debug.Log("No ground detected");
            nextMove *= -1;
            Flip();
        }
    }

    private void WallDetector()
    {
        Vector2 checkPos = rigid.position + new Vector2(nextMove * 0.5f, 0);
        Debug.DrawRay(checkPos, Vector2.right * nextMove * 1.5f, Color.blue);

        if (Physics2D.Raycast(checkPos, Vector2.right * nextMove, 1.5f, wallLayer))
        {
            Debug.Log("Wall detected");
            nextMove *= -1;
            Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
