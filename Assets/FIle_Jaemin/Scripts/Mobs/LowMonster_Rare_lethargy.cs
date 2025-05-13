using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LowMonster_Rare_lethargy : Monster
{
    public float attackRadius;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        StartCoroutine(Explosion(attackCoolDown));
    }

    IEnumerator Explosion(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.tag == "Player")
            {
                Debug.Log("Explosion");
                collidersEnemies[i].GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
        gameObject.SetActive(false);
    }

    protected override void Die()
    {
        Attack();
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        
    }
}
