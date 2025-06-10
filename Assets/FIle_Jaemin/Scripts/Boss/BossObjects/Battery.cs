using System;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform explosionTransform;
    [SerializeField] private float damage;

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void Die()
    {
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(explosionTransform.position, attackRadius);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.tag == "Boss")
            {
                Frustration frustration = collidersEnemies[i].GetComponent<Frustration>();
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
