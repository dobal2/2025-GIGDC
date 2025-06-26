using UnityEngine;

public class TargetBoard : Monster
{
    private Boss boss;
    private int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //화살 태그나 레이어
        // if (collision.CompareTag("Arrow"))
        // {
        //     if (boss != null)
        //     {
        //         boss.TakeDamage(damage);
        //     }
        // }
    }

    public void SetBoss(Boss be)
    {
        boss = be;
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public override void TakeDamage(float amount)
    {
        boss.TakeDamage(amount);
    }

    protected override void Attack()
    {
        
    }
}