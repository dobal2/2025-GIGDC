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


    [SerializeField] protected Transform player;
    protected Rigidbody2D rigid;
    protected Animator anim;
    public bool facingRight = false;
    protected bool canAttack = true;

    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
        GameObject newInkExplosion = Instantiate(inkDeathEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Destroy(newInkExplosion,2);
    }

    public virtual void TakeDamage(float amount)
    {
        hp -= amount;
        GameObject newInkExplosion = Instantiate(inkHitEffect, transform.position, Quaternion.identity);
        Destroy(newInkExplosion,2);

        // 공격 중이라면 끊기
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;

            anim.ResetTrigger("Attack");
            anim.Play("Idle");
        }

        TakeDamageAnimation();

        if (hp <= 0) Die();
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
        if (anim.GetBool("IsWalking"))
        {
            anim.SetBool("isWalking", false);
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
    
}
