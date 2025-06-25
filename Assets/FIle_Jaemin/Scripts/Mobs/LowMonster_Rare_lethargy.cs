using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LowMonster_Rare_lethargy : Monster
{
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform explosionTransform;
    


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
        anim.SetBool("IsCharging",true);
        
        yield return new WaitForSeconds(delayTime);
        
        anim.SetTrigger("Explode");
        
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(explosionTransform.position, attackRadius);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.tag == "Player")
            {
                Debug.Log("Explosion");
                collidersEnemies[i].GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }

    protected override void Die()
    {
        Attack();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public override void TakeDamage(float amount)
    {
        hp -= amount;
        TakeDamageAnimation();
        if (hp <= 0) Die();
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
