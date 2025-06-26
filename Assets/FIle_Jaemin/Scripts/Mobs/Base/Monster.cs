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
    [SerializeField] protected GameObject inkExplosion;
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

    protected virtual void Die()
    {
        GameObject newInkExplosion = Instantiate(inkExplosion, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Destroy(newInkExplosion,2);
    }

    public virtual void TakeDamage(float amount)
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
        KnockBack();

        if (hp <= 0) Die();
    }

    protected IEnumerator DoStun(float duration)
    {
        isStunned = true;
        canAttack = false;
        if (anim.GetBool("IsWalking"))
        {
            anim.SetBool("isWalking", false);   
        }
        rigid.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(duration);

        isStunned = false;
        canAttack = true;
    }

    
    protected void KnockBack()
    {
        float knockBackForce = 5f;
        Vector2 direction = Vector2.zero;
        
        float yRotation = player.transform.eulerAngles.y;

        if (Mathf.Approximately(yRotation, 0f))
        {
            direction = Vector2.right;
        }
        else if (Mathf.Approximately(yRotation, 180f))
        {
            direction = Vector2.left;
        }
        
        rigid.linearVelocity = Vector2.zero;
        Vector2 force = direction.normalized * knockBackForce;

        rigid.AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(StopKnockBack());
    }

    IEnumerator StopKnockBack()
    {
        yield return new WaitForSeconds(0.2f);
        rigid.linearVelocity = Vector2.zero;
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
