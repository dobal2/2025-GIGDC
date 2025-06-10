using System;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;
    [SerializeField] protected float phase2Hp;
    [SerializeField] protected float damage;
    protected int currentPhase = 1;
    
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

    protected virtual void Update()
    {
        if (hp <= 0) Die();
    }

    protected abstract void Die();
    
    public virtual void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0) Die();
    }
    
    protected void Flip()
    {
        facingRight = !facingRight;
        
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + 180f, 0f);
    }


}
