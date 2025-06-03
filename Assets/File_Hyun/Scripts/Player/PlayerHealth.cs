using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    public float maxHealth;
    [SerializeField] private float invincibleTime = 1f;

    [HideInInspector] public bool isInvincible;
    private float health;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        health = maxHealth;
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        gameObject.SetActive(false);
    }
    
    public void TakeDamage(float damage)
    {
        if (!PlayerController.Instance.CanTakeDamage)
        {
            Debug.Log("Player is invincible. No damage taken.");
            return;
        }

        health -= damage;
        Debug.Log("Now Player Health: "+health);

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
