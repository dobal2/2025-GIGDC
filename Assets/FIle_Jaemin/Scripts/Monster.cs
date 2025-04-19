using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackCoolDown;

    protected abstract void Move();

    protected abstract void Attack();

    protected abstract void Die();

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }
    
}
