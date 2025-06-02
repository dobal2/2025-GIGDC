using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public float health;
    [SerializeField] private float invincibleTime = 1;
    [SerializeField] private bool isInvincible;

    private void Die()
    {
        gameObject.SetActive(false);
    }
    
    public void TakeDamage(float damage)
    {
        if(isInvincible)
            return;
        
        health -= damage;
        Debug.Log("Now Player Health "+health);

        if (health <= 0)
        {
            Die();
            return;
        }

        isInvincible = true;
        StartCoroutine(ResetInvincible());
    }

    private IEnumerator ResetInvincible()
    {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
}
