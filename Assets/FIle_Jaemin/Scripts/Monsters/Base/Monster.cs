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

    [SerializeField] protected Transform player;
    private Animator anim;
    protected bool facingRight = true;
    protected bool canAttack = true;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected abstract void Attack();
    protected abstract void Die();

    public virtual void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0) Die();
    }
    
    protected void Flip()
    {
        facingRight = !facingRight;
        
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    protected IEnumerator WaitToAttack(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time);
        canAttack = true;
    }
    
}
