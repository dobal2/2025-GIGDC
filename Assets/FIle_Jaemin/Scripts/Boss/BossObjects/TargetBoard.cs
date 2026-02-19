using UnityEngine;

public class TargetBoard : Monster
{
    [SerializeField] private Boss_Excitement boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }
    
    

    public void SetBoss(Boss_Excitement be)
    {
        boss = be;
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public override void TakeDamage(float amount)
    {
        boss.TakeDamage(amount,true);
    }

    public override void KnockBack(Transform attacker, float knockBackForce, float knockBackAngle, float duration)
    {
        
    }

    protected override void Attack()
    {
        
    }
}