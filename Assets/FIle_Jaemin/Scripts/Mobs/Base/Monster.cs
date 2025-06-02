using System;
using System.Collections;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    [Header("Stats")] [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackCoolDown;

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

    public virtual void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0) Die();
    }
    
    public void KnockBack(bool isRightAttack, float knockBackForce,bool doTakeDamageAnimation)
    {
        if (doTakeDamageAnimation)
        {
            TakeDamageAnimation();
        }
        
        rigid.linearVelocity = Vector2.zero;
        
        if (isRightAttack)
        {
            rigid.AddForce(new Vector2(knockBackForce, 0),ForceMode2D.Impulse);
        }
        else
        {
            rigid.AddForce(new Vector2(-knockBackForce, 0),ForceMode2D.Impulse);
        }
    }

    public void TakeDamageAnimation()
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
