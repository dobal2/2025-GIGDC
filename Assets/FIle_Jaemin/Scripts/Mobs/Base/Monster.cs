using System;
using System.Collections;
using UnityEngine;
using TMPro;

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
    protected bool isStunned = false;
    protected Coroutine attackCoroutine;
    protected Coroutine stunCoroutine;
    protected bool isDead = false;
    
    [Header("Counter System")]
    protected bool isCountering = false;
    protected bool isCounterStunned = false;
    protected bool isCounterAttack = false;
    protected bool canCounter = true;
    protected Coroutine counterCoroutine;
    protected Coroutine counterStunCoroutine;
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;
    [SerializeField] protected GameObject counterTextPrefab;
    protected TextMeshPro counterText;

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

        if (counterTextPrefab != null && counterText == null)
        {
            GameObject textObj = Instantiate(counterTextPrefab, transform);
            textObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            counterText = textObj.GetComponent<TextMeshPro>();

            if (counterText != null)
            {
                counterText.sortingOrder = 100;
                counterText.gameObject.SetActive(false);
            }
        }
    }

    protected abstract void Attack();

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        
        StageManager.Objects--;
        GameObject newInkExplosion = Instantiate(inkDeathEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Destroy(newInkExplosion, 2);
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        if (isCounterStunned)
        {
            amount *= 1.5f;
        }

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            anim.ResetTrigger("Attack");
            anim.Play("Idle");
        }

        hp -= amount;
        GameObject newInkExplosion = Instantiate(inkHitEffect, transform.position, Quaternion.identity);
        Destroy(newInkExplosion, 2);

        if (hp <= 0)
        {
            Die();
            return;
        }

        TakeDamageAnimation();
    }

    public void SetHealth(int health)
    {
        hp = health;
    }

    public virtual bool OnCounterHit()
    {
        if (!isCountering) return false;

        if (counterCoroutine != null)
        {
            StopCoroutine(counterCoroutine);
            counterCoroutine = null;
        }

        isCountering = false;

        if (anim != null)
            anim.SetBool("IsCountering", false);

        // if (counterText != null)
        // {
        //     counterText.gameObject.SetActive(false);
        // }

        if (counterStunCoroutine != null)
            StopCoroutine(counterStunCoroutine);

        counterStunCoroutine = StartCoroutine(CounterStunRoutine());
        return true;
    }

    protected virtual void LateUpdate()
    {
        if (counterText != null)
        {
            counterText.transform.rotation = Quaternion.identity;
        }

    }

    public virtual void KnockBack(Transform attacker, float knockBackForce, float knockBackAngle, float duration)
    {
        if (isDead || rigid == null) return;
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
    
    protected bool TryCounter()
    {
        if (!canCounter || attackCoroutine != null || isCountering || isCounterStunned || isStunned)
            return false;

        StartCounter();
        return true;
    }
    
    public void StartCounter()
    {
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

        if (anim != null)
            anim.SetBool("IsCountering", true);

        // if (counterText != null)
        // {
        //     counterText.gameObject.SetActive(true);
        // }

        yield return new WaitForSeconds(1.5f);

        isCountering = false;

        if (anim != null)
            anim.SetBool("IsCountering", false);

        // if (counterText != null)
        // {
        //     counterText.gameObject.SetActive(false);
        // }

        counterCoroutine = null;
        canCounter = true;

        isCounterAttack = true;
        Attack();
    }
    
    protected IEnumerator CounterStunRoutine()
    {
        isCounterStunned = true;
        canAttack = false;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.gray;
        }
        
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
        
        isCounterStunned = false;
        canAttack = true;
        canCounter = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        counterStunCoroutine = null;
    }
}