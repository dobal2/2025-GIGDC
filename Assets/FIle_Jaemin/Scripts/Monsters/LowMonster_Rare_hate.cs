using System.Collections;
using UnityEngine;

public class LowMonster_Rare_hate : Monster
{
    [SerializeField] private float playerNoticeDistance = 5f;
    [SerializeField] private float windForce = 10f;

    private int nextMove;

    private Rigidbody2D playerRb;

    protected override void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
        if(playerRb == null)
            Debug.LogError("Player has no Rigidbody");
    }

    private void Update()
    {
        if (canAttack)
        {
            Attack();
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
    

    private void ApplyWindEffect()
    {
        if (playerRb != null)
        {
            Vector2 windDirection = Vector2.zero;
            
            if (facingRight)
                windDirection = Vector2.left;
            else
                windDirection = Vector2.right;
            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(windDirection.normalized * windForce,ForceMode2D.Impulse);
        }
    }

    protected override void Attack()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < playerNoticeDistance)
        {
            Debug.Log("WindAttack");
            ApplyWindEffect();
            StartCoroutine(WaitToAttack(attackCoolDown));
        }
    }
    

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
