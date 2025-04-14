using UnityEngine;


public class LowMonster_Common_regret : LowMonster
{
    private Animator anim;
    private Rigidbody2D rigid;
    public int nextMove;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        SetRandomMoveDirection();
    }

    private void FixedUpdate()
    {
        Move();
        
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
        rigid.linearVelocity = new Vector2(speed * nextMove, rigid.linearVelocity.y);
        
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
            Debug.Log("NoGround");
            nextMove *= -1;
        }
    }
    
    private void WallDetector()
    {
        Vector2 frontVec = rigid.position + new Vector2(nextMove * 0.5f, 0);
            
        Debug.DrawRay(frontVec, new Vector2(nextMove, 0) * 0.5f, Color.blue);
            
        RaycastHit2D wallRayHit = Physics2D.Raycast(frontVec, new Vector2(nextMove, 0), 0.5f, wallLayer);

        if (wallRayHit.collider != null)
        {
            Debug.Log("Wall Detected");
            nextMove *= -1;
        }

    }

    protected override void Attack()
    {
        
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
