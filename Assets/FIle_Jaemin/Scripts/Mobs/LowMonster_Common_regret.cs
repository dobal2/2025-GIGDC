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
    
    
    protected override void Start()
    {
        base.Start();
        SetRandomMoveDirection();
    }

    private void FixedUpdate()
    {
        Move();

        float distance = Vector2.Distance(player.position, transform.position);
        
        if (Mathf.Abs(distance) <= stopDistance)
        {
            canMove = false;
            
            if (canAttack)
            {
                if (player.transform.position.x > transform.position.x && !facingRight)
                {
                    Flip();
                    nextMove = 1;
                }
                else if(player.transform.position.x < transform.position.x && facingRight)
                {
                    Flip();
                    nextMove = -1;
                }
                Attack();
            }
        }
        else
        {
            canMove = true;
        }
        
        if (nextMove == -1)
        {
            if (facingRight)
            {
                Flip();
            }
        }
        else if (nextMove == 1)
        {
            if (!facingRight)
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

    void Update()
    {
        
    }

    private void Move()
    {
        if (canMove)
        {
            Debug.Log("Move");
            rigid.linearVelocity = new Vector2(speed * nextMove, rigid.linearVelocity.y);
            anim.SetBool("isWalking",true);
        }
        else
        {
            anim.SetBool("isWalking",false);
        }

        GroundDetector();
        WallDetector();

    }

    private void GroundDetector()
    {
        Vector2 frontVec = new Vector2((rigid.position.x + nextMove), rigid.position.y);
        Debug.DrawRay(frontVec,Vector3.down,new Color(0,1,0));
        RaycastHit2D groundRayHit = Physics2D.Raycast(frontVec, Vector3.down,1,groundLayer);
        if (groundRayHit.collider == null)
        {
            Debug.Log("NoGround Detected");
            nextMove *= -1;
        }
    }
    
    private void WallDetector()
    {
        Vector2 frontVec = rigid.position + new Vector2(nextMove * 0.5f, 0);
            
        Debug.DrawRay(frontVec, new Vector2(nextMove, 0) * 0.5f, Color.blue);
            
        RaycastHit2D wallRayHit = Physics2D.Raycast(frontVec, new Vector2(nextMove, 0), 0.5f,wallLayer);

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
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackRadius);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.tag == "Player")
            {
                Debug.Log("Attack");
                collidersEnemies[i].GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
        StartCoroutine(WaitToAttack(attackCoolDown));    
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
