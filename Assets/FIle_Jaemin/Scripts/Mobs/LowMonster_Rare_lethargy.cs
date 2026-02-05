using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class LowMonster_Rare_lethargy : Monster
{
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform explosionTransform;
    private AudioSource explosionSound;
    public bool isExplosioned = false;
    


    protected override void Start()
    {
        base.Start();
        explosionSound = GetComponent<AudioSource>();
    }

    protected override void Attack()
    {
        StartCoroutine(Explosion(attackCoolDown));
    }

    public IEnumerator Explosion(float delayTime)
    {
        if(isExplosioned)
            yield break;
        isExplosioned = true;
        anim.SetBool("IsCharging",true);
        
        yield return new WaitForSeconds(delayTime);
        
        explosionSound.Play();
        
        anim.SetTrigger("Explode");
        GameObject newInkExplosion = Instantiate(inkDeathEffect, transform.position, Quaternion.identity);
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
        StageManager.Objects--;
    }

    public override void TakeDamage(float amount)
    {
        if(hp <= 0)
            return;
        
        GameObject newInkExplosion;

        if (isCountering)
        {
            if (counterCoroutine != null)
            {
                StopCoroutine(counterCoroutine);
                counterCoroutine = null;
            }
            
            isCountering = false;

            hp -= amount;
            newInkExplosion = Instantiate(inkHitEffect, transform.position, Quaternion.identity);
            Destroy(newInkExplosion,2);
            
            if (hp <= 0)
            {
                Die();
                return;
            }

            if (counterStunCoroutine != null)
            {
                StopCoroutine(counterStunCoroutine);
            }
            counterStunCoroutine = StartCoroutine(CounterStunRoutine());
            return;
        }

        if (isCounterStunned)
        {
            amount *= 1.5f;
        }
        
        newInkExplosion = Instantiate(inkHitEffect, transform.position, Quaternion.identity);
        Destroy(newInkExplosion,2);
        hp -= amount;
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
