using System.Collections;
using UnityEngine;

public class LowMonster_Rare_hate : Monster
{
    [SerializeField] private float playerNoticeDistance = 5f;
    [SerializeField] private float windForce = 10f;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (canAttack)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < playerNoticeDistance)
            {
                Attack();   
            }
        }
        
        bool shouldFlip = (player.position.x > transform.position.x) != facingRight;
        if (shouldFlip)
        {
            Flip();
        }
    }
    

    private void ApplyWindEffect()
    {
        player.GetComponent<PlayerHealth>().TakeDamage(damage);
        Rigidbody2D playerRigid = player.GetComponent<Rigidbody2D>();
        if (playerRigid != null)
        {
            Vector2 windDirection = Vector2.zero;
            
            if (facingRight)
                windDirection = Vector2.right;
            else
                windDirection = Vector2.left;
            playerRigid.linearVelocity = Vector2.zero;
            playerRigid.AddForce(windDirection.normalized * windForce,ForceMode2D.Impulse);
        }
        else
        {
            Debug.Log("playerRigid null");
        }
    }

    protected override void Attack()
    {
        anim.SetTrigger("Attack");
        //ApplyWindEffect();
        StartCoroutine(WaitToAttack(attackCoolDown));
    }
    

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
