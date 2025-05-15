  using System;
  using System.Collections;
  using UnityEngine;

public class LowMonster_Common_sad : Monster
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject firePos;

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
        anim.SetTrigger("Attack");
        StartCoroutine(Fire());
        
        StartCoroutine(WaitToAttack(attackCoolDown));    
    }
    

    IEnumerator Fire()
    {
        yield return new WaitForSeconds(1.4f);
        
        Debug.Log("Sad Fire");
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePos.transform.position, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        projectile.transform.rotation = Quaternion.Euler(0,0,angle-30);
        
        Bullet bullet = projectile.GetComponent<Bullet>();
        
        bullet.damage = damage;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
    }

    
    
    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
