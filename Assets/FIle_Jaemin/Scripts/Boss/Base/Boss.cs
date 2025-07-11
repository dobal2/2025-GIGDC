using System;
using UnityEngine;

public abstract class Boss : Monster
{

    [SerializeField] protected float phase2Hp;
    protected int currentPhase = 1;
    protected bool battleStarted = false;
    
    [SerializeField] protected RuntimeAnimatorController phase2Anim;

    public static Boss Instance { get; private set; }

    public void StartBattle()
    {
        battleStarted = true;
    }
    
    
    protected override void Start()
    {
        Instance = this;
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

    public override void KnockBack(Transform attacker, float knockBackForce, float knockBackAngle, float duration)
    {
        
    }


    protected virtual void Update()
    {
        if (hp <= 0) Die();
    }

    protected abstract override void Die();
    
    public override void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0) Die();
    }
    


}
