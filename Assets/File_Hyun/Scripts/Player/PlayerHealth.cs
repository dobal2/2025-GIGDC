using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    public float MaxHealth;
    public float CurrentHealth;
    [SerializeField] private float invincibleTime = 1f;

    [HideInInspector] public bool isInvincible;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Die()
    {
        PlayerController.Instance.Die();
    }
    
    public void TakeDamage(float damage)
    {
        if (!PlayerController.Instance.CanTakeDamage)
        {
            Debug.Log("Player is invincible. No damage taken.");
            return;
        }

        CurrentHealth -= damage;
        Debug.Log("Now Player Health: "+CurrentHealth);

        if (CurrentHealth <= 0)
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
