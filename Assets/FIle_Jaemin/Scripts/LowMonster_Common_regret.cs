
using System.Collections;
using UnityEngine;


public class LowMonster_Common_regret : LowMonster
{
    public GameObject player;
    private Animator anim;
    private Rigidbody2D rigid;
    public int nextMove;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public Transform attackCheck;
    public float attackRadius;
    private bool facingRight = true;
    private bool canAttack = true;
    private bool canMove = true;
    
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        SetRandomMoveDirection();
    }

    private void FixedUpdate()
    {
        Move();

        float distance = Vector2.Distance(player.transform.position, transform.position);
        
        if (Mathf.Abs(distance) <= 3f)
        {
            canMove = false;
            if (canAttack)
            {
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

    protected override void Move()
    {
        if (canMove)
        {
            rigid.linearVelocity = new Vector2(speed * nextMove, rigid.linearVelocity.y);
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
    
    IEnumerator WaitToAttack(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
    
    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
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
