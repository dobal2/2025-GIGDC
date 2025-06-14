using System;
using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float attackRadius;
    public float damage;

    private void Start()
    {
        StartCoroutine(Explosion(5f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Explosion(0.2f));
        }
    }
    
    IEnumerator Explosion(float delayTime)
    {
        
        yield return new WaitForSeconds(delayTime);
        
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

    public void TakeHit(float damage)
    {
        Destroy(gameObject);
    }
}
