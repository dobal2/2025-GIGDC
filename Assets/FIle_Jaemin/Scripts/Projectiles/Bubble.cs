using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Bubble : Monster
{
    [SerializeField] protected Boss_Love boss;
    [SerializeField] protected float attackRadius;
    [SerializeField] protected GameObject bubblePopEffect;
    [SerializeField] protected float bossTakeDamage;


    public void SetBoss(Boss_Love newBoss)
    {
        boss = newBoss;
    }

    protected override void Start()
    {
        StartCoroutine(Explosion(5));
    }

    protected override void Attack()
    {
        
    }
    
    
    public IEnumerator Explosion(float delayTime)
    {
        
        yield return new WaitForSeconds(delayTime);
        
        PlayBubblePopEffect();
        
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.CompareTag("Player"))
            {
                Debug.Log("Explosion");
                collidersEnemies[i].GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
        
        Destroy(gameObject);
    }

    protected void PlayBubblePopEffect()
    {
        VisualEffect newPop = Instantiate(bubblePopEffect, transform.position, Quaternion.identity).GetComponent<VisualEffect>();
        
        newPop.Play();
        
        Destroy(newPop,2);
    }

    public override void TakeDamage(float amount)
    {
        if (boss != null)
        {
            
        }
        hp -= amount;
        Die();
    }

    protected override void Die()
    {
        
        PlayBubblePopEffect();
        Destroy(gameObject);
    }

    public override void KnockBack(Transform attacker, float knockBackForce, float knockBackAngle, float duration)
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
    
    
}
