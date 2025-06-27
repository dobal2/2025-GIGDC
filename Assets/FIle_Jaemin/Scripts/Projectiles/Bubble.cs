using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Bubble : Monster
{
    [SerializeField] private float attackRadius;
    [SerializeField] private GameObject bubblePopEffect;

    protected override void Start()
    {
        Destroy(gameObject,5);
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

    private void PlayBubblePopEffect()
    {
        VisualEffect newPop = Instantiate(bubblePopEffect, transform.position, Quaternion.identity).GetComponent<VisualEffect>();
        
        newPop.Play();
        
        Destroy(newPop,2);
    }

    public override void TakeDamage(float amount)
    {
        hp -= amount;
        Die();
    }

    protected override void Die()
    {
        PlayBubblePopEffect();
        Destroy(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
    
    
}
