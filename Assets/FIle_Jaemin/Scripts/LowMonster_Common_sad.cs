  using System;
  using System.Collections;
  using UnityEngine;

public class LowMonster_Common_sad : LowMonster
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float bulletSpeed;
    
    private bool canAttack = true;

    private void FixedUpdate()
    {
        if (canAttack)
        {
            Attack();
        }
    }

    protected override void Move()
    {
        throw new System.NotImplementedException();
    }

    protected override void Attack()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Bullet bullet = projectile.GetComponent<Bullet>();
        
        bullet.damage = damage;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
        
        StartCoroutine(WaitToAttack(attackCoolDown));    
        
        
    }
    
    IEnumerator WaitToAttack(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
