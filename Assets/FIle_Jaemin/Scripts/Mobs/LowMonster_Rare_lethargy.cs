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
        GameObject newInkExplosion = Instantiate(inkExplosion, transform.position, Quaternion.identity);
        Destroy(newInkExplosion,1);
        
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

    }

    protected override void Die()
    {
        Attack();
    }

    public void DestroyObject()
    {
        gameObject.SetActive(false);
    }

    public override void TakeDamage(float amount)
    {
        if(hp <= 0)
            return;
        hp -= amount;
        //TakeDamageAnimation();
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
