using System.Collections;
using UnityEngine;

public class LowMonster_Rare_hate : Monster
{
    [SerializeField] private float playerNoticeDistance = 5f;
    [SerializeField] private float windForce = 10f;

    private int nextMove;

    protected override void Start()
    {
        base.Start();
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
        if (rigid != null)
        {
            Vector2 windDirection = Vector2.zero;
            
            if (facingRight)
                windDirection = Vector2.left;
            else
                windDirection = Vector2.right;
            rigid .linearVelocity = Vector2.zero;
            rigid .AddForce(windDirection.normalized * windForce,ForceMode2D.Impulse);
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
