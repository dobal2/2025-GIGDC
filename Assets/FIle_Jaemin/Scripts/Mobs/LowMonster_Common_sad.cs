  using System;
  using System.Collections;
  using UnityEngine;

public class LowMonster_Common_sad : Monster
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject firePos;
    
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        if(projectilePrefab == null)
            Debug.LogError("No projectile prefab assigned");
    }

    private void FixedUpdate()
    {
        if (isStunned || isCounterStunned) return;
        
        FacePlayer();
        
        if (canAttack && !isCountering)
        {
            // 카운터 시도, 실패하면 공격
            if (!TryCounter())
            {
                Attack();
            }
        }
    }
    
    private void FacePlayer()
    {
        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            Flip();
        }
    }
    protected override void Attack()
    {
        if (isAttacking)
            return;
        isAttacking = true;
        anim.SetTrigger("Attack");
        
        StartCoroutine(WaitToAttack(attackCoolDown));    
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    public override void TakeDamage(float amount)
    {
        GameObject newInkExplosion;
        
        // 카운터 중이라면 카운터 중단하고 즉시 기절 상태로 전환
        if (isCountering)
        {
            if (counterCoroutine != null)
            {
                StopCoroutine(counterCoroutine);
                counterCoroutine = null;
            }
            
            isCountering = false;
            
            // 데미지 적용
            hp -= amount;
            newInkExplosion = Instantiate(inkHitEffect, transform.position, Quaternion.identity);
            Destroy(newInkExplosion,2);
            
            if (hp <= 0)
            {
                Die();
                return;
            }
            
            // 공격 중이라면 끊기
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
                isAttacking = false;

                anim.ResetTrigger("Attack");
                anim.Play("Idle");
            }
            
            // 기절 코루틴 시작하고 바로 종료 (일반 피격 처리 건너뜀)
            if (counterStunCoroutine != null)
            {
                StopCoroutine(counterStunCoroutine);
            }
            counterStunCoroutine = StartCoroutine(CounterStunRoutine());
            return;  // 카운터 피격은 여기서 종료
        }
        
        // 카운터 기절 중이면 대미지 50% 증가 (1.5배)
        if (isCounterStunned)
        {
            amount *= 1.5f;
        }
        
        hp -= amount;
        newInkExplosion = Instantiate(inkHitEffect, transform.position, Quaternion.identity);
        Destroy(newInkExplosion,2);

        // 공격 중이라면 끊기
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            isAttacking = false;

            anim.ResetTrigger("Attack");
            anim.Play("Idle");
        }

        if (hp <= 0) Die();
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(attackCoolDown);
        attackCoroutine = null;
        isAttacking = false;
    }

    public void Fire()
    {
        Debug.Log("Sad Fire");
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePos.transform.position, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        projectile.transform.rotation = Quaternion.Euler(0,0,angle-30);
        
        Bullet bullet = projectile.GetComponent<Bullet>();
        
        bullet.damage = damage;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
    }

    
    
    protected override void Die()
    {
        base.Die();
    }
}
