  using System;
  using System.Collections;
  using UnityEngine;

public class LowMonster_Common_sad : Monster
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float bulletSpeed;

    protected override void Start()
    {
        base.Start();
        if(projectilePrefab == null)
            Debug.LogError("No projectile prefab assigned");
    }

    private void FixedUpdate()
    {
        if (canAttack)
        {
            Attack();
        }
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
    
    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
