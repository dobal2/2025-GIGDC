using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public float health;

    private void Die()
    {
        gameObject.SetActive(false);
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }
}
