using System;
using System.Collections;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackCoolDown;
    [SerializeField] protected GameObject inkHitEffect;
    [SerializeField] protected GameObject inkDeathEffect;
    protected bool isStunned = false; // 경직 상태
    protected Coroutine attackCoroutine;
    protected Coroutine stunCoroutine;
    
    [Header("Counter System")]
    [SerializeField] protected float counterChance = 0.3f; // 카운터 확률 (30%)
    [SerializeField] protected float counterCooldown = 5f; // 카운터 쿨타임 (5초)
    protected bool isCountering = false; // 카운터 대기 상태
    protected bool isCounterStunned = false; // 카운터 실패 기절 상태
    protected bool canCounter = true; // 카운터 쿨타임 체크
    protected Coroutine counterCoroutine;
    protected Coroutine counterStunCoroutine;
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;


    [SerializeField] protected Transform player;
    protected Rigidbody2D rigid;
    protected Animator anim;
    public bool facingRight = false;
    protected bool canAttack = true;

    protected virtual void Awake()
    {
        StageManager.Objects++;
    }

    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        if (anim != null)
        {
            Debug.Log("Anim not null");
        }
        else
        {
            Debug.Log("Anim null");
        }

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("No Player");
        }
    }

    protected abstract void Attack();

    protected virtual void Die()
    {
        //StageManager.Objects--;
        GameObject newInkExplosion = Instantiate(inkDeathEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Destroy(newInkExplosion,2);
    }

    public virtual void TakeDamage(float amount)
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
        
        if (hp <= 0)
        {
            Die();
            return;  // 여기서 즉시 종료
        }


        // 공격 중이라면 끊기
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;

            anim.ResetTrigger("Attack");
            anim.Play("Idle");
        }

        TakeDamageAnimation();
        
    }

    public virtual void KnockBack(Transform attacker, float knockBackForce, float knockBackAngle, float duration)
    {
        rigid.linearVelocity = Vector2.zero;

        bool isRight = transform.position.x > attacker.position.x;
        float angle = isRight ? knockBackAngle : 180f - knockBackAngle;

        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        direction.Normalize();

        Vector2 force = direction * knockBackForce;

        rigid.AddForce(force, ForceMode2D.Impulse);

        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(DoStun(duration));
    }

    protected IEnumerator DoStun(float duration)
    {
        isStunned = true;
        canAttack = false;
        if (anim != null && anim.GetBool("IsWalking"))
        {
            anim.SetBool("IsWalking", false);
        }

        yield return new WaitForSeconds(duration);

        isStunned = false;
        canAttack = true;
    }

    protected void TakeDamageAnimation()
    {
        anim.SetTrigger("Hit");
    }

    protected void Flip()
    {
        facingRight = !facingRight;
        
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + 180f, 0f);
    }


    protected IEnumerator WaitToAttack(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time);
        canAttack = true;
    }
    
    // 카운터 시스템 메서드들
    // 카운터를 시도할지 결정하는 메서드 (공격 전에 호출)
    protected bool TryCounter()
    {
        // 카운터 조건 확인
        if (!canCounter || attackCoroutine != null || isCountering || isCounterStunned || isStunned)
            return false;
        
        // 랜덤 확률로 카운터 시도
        if (UnityEngine.Random.value <= counterChance)
        {
            StartCounter();
            return true;
        }
        
        return false;
    }
    
    public void StartCounter()
    {
        // 공격 중이거나 이미 카운터 중이거나 기절 중이면 카운터 불가
        if (attackCoroutine != null || isCountering || isCounterStunned || isStunned || !canCounter)
            return;
            
        if (counterCoroutine != null)
            StopCoroutine(counterCoroutine);
            
        counterCoroutine = StartCoroutine(CounterRoutine());
    }
    
    protected IEnumerator CounterRoutine()
    {
        isCountering = true;
        canCounter = false;
        
        // 빨강색으로 변경
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
        
        yield return new WaitForSeconds(1.5f);
        
        // 카운터 대기 종료
        isCountering = false;
        
        // 원래 색상으로 복원
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        counterCoroutine = null;
        
        // 카운터 쿨타임 시작
        yield return new WaitForSeconds(counterCooldown);
        canCounter = true;
    }
    
    protected IEnumerator CounterStunRoutine()
    {
        isCounterStunned = true;
        canAttack = false;
        
        // 회색으로 변경
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.gray;
        }
        
        // 공격 중단
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            
            if (anim != null)
            {
                anim.ResetTrigger("Attack");
                anim.Play("Idle");
            }
        }
        
        yield return new WaitForSeconds(1.5f);
        
        // 기절 종료
        isCounterStunned = false;
        canAttack = true;
        
        // 원래 색상으로 복원
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        counterStunCoroutine = null;
    }
    
}
