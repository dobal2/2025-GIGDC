using System.Collections;
using UnityEngine;

public class LowMonster_Rare_hate : Monster
{
    [SerializeField] private float playerNoticeDistance = 5f;
    [SerializeField] private float windForce = 10f;
    [SerializeField] private GameObject windParticle;
    [SerializeField] private Transform attackTransform;
    [SerializeField] private Vector2 attackSize;

    private bool isAttacking;

    protected override void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        if (canAttack && !isAttacking)
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

        if (isAttacking)
        {
            CheckDashHitbox();
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
        isAttacking = true;
        anim.SetTrigger("Attack");
        ParticleSystem particle = windParticle.GetComponent<ParticleSystem>();
        particle.Play();
        
        StartCoroutine(WaitToAttack(attackCoolDown));
    }
    
    private void CheckDashHitbox()
    {
        Collider2D[] collidersEnemies = Physics2D.OverlapBoxAll(attackTransform.position, attackSize, 0);
        foreach (var collider in collidersEnemies)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }
    
    protected override void Die()
    {
        base.Die();
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackTransform != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawCube(attackTransform.position, attackSize);
        }
    }
}
