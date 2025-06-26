using System;
using UnityEngine;
using UnityEngine.VFX;

public class Battery : Monster
{
    [SerializeField] private float health;
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform explosionTransform;
    [SerializeField] private float damage;
    [SerializeField] private GameObject explosionEffectPrefab;
    

    public override void TakeDamage(float amount)
    {
        health -= damage;
    }

    protected override void Attack()
    {
        
    }

    protected override void Die()
    {
        GameObject newExplosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        newExplosionEffect.GetComponent<VisualEffect>().Play();
        
        Destroy(newExplosionEffect,2);
        
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(explosionTransform.position, attackRadius);
        
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.tag == "Boss")
            {
                Boss_Frustration frustration = collidersEnemies[i].GetComponent<Boss_Frustration>();
                if (frustration)
                {
                    Debug.Log("Explosion");   
                    frustration.TakeDamage(true,damage);
                }
            }
        }
        
        Destroy(gameObject);
    }

    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (explosionTransform != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(explosionTransform.position, attackRadius);   
        }
        
    }
}
