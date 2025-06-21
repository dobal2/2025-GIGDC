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
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No Player");
        }
    }

    protected abstract void Attack();
    protected abstract void Die();

    public virtual void TakeDamage(float amount, Vector2 knockBackDir)
    {
        hp -= amount;

        // 공격 중이라면 끊기
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;

            anim.ResetTrigger("Attack");
            anim.Play("Idle");
        }

        // 경직 시작
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(DoStun(0.5f)); // 예: 0.5초 경직

        TakeDamageAnimation();
        KnockBack(knockBackDir);

        if (hp <= 0) Die();
    }

    protected IEnumerator DoStun(float duration)
    {
        isStunned = true;
        canAttack = false;
        anim.SetBool("isWalking", false);
        rigid.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(duration);

        isStunned = false;
        canAttack = true;
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("TakeDamage 테스트");
            TakeDamage(10f, new Vector2(-1f, 1f)); // 왼쪽 위 방향 넉백
        }
    }


    
    protected void KnockBack(Vector2 direction)
    {
        rigid.linearVelocity = Vector2.zero;

        float knockBackForce = 5f;
        Vector2 force = direction.normalized * knockBackForce;

        rigid.AddForce(force, ForceMode2D.Impulse);
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
