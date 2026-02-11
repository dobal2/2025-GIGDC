using System.Collections;
using UnityEngine;

public class LowMonster_Rare_hate : Monster
{
    [SerializeField] private float playerNoticeDistance = 5f;
    [SerializeField] private float windForce = 10f;
    [SerializeField] private GameObject windParticle;
    [SerializeField] private Transform attackTransform;
    [SerializeField] private Vector2 attackSize;
    private bool canFlip = true;
    
    private Coroutine delayCoroutine;

    private bool isAttacking;

    protected override void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        if (!isAttacking && !isCountering && !isCounterStunned)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < playerNoticeDistance)
            {
                TryCounter();
            }
        }

        if (canFlip && !isCountering && !isCounterStunned)
        {
            bool shouldFlip = (player.position.x > transform.position.x) != facingRight;
            if (shouldFlip)
            {
                Flip();
            }
        }
        

        if (isAttacking)
        {
            CheckDashHitbox();
        }
    }
    

    private void ApplyWindEffect()
    {
        Vector2 windDirection = Vector2.zero;
            
        if (facingRight)
            windDirection = Vector2.right;
        else
            windDirection = Vector2.left;
        PlayerController.Instance.ApplyKnockback(windDirection.normalized, windForce);
        Debug.Log("ApplyWindEffect");
    }

    protected override void Attack()
    {
        if (attackCoroutine != null || isStunned) return;

        canFlip = false;
        anim.SetBool("IsAttacking",true);
        
        attackCoroutine = StartCoroutine(AttackRoutine());
        
        delayCoroutine =  StartCoroutine(DelayToDisableIsAttacking());
    }
    
    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(attackCoolDown);
        attackCoroutine = null;
    }

    public void ActiveAttacking()
    {
        isAttacking = true;
        ParticleSystem particle = windParticle.GetComponent<ParticleSystem>();
        particle.Play();
    }

    IEnumerator DelayToDisableIsAttacking()
    {
        yield return new WaitForSeconds(2);
        isAttacking = false;
        ParticleSystem particle = windParticle.GetComponent<ParticleSystem>();
        particle.Stop();
        anim.SetBool("IsAttacking",false);
        anim.SetTrigger("AttackEnd");

        yield return new WaitForSeconds(0.5f);
        canFlip = true;
    }
    
    private void CheckDashHitbox()
    {
        Collider2D[] collidersEnemies = Physics2D.OverlapBoxAll(attackTransform.position, attackSize, 0);
        foreach (var collider in collidersEnemies)
        {
            if (collider.CompareTag("Player"))
            {
                ApplyWindEffect();
                collider.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        anim.SetBool("IsAttacking",false);
        if (delayCoroutine != null)
        {
            canFlip = true;
            isAttacking = false;
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
            ParticleSystem particle = windParticle.GetComponent<ParticleSystem>();
            particle.Stop();
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
