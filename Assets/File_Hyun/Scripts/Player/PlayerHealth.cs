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
    
    public void TakeDamage(float amount)
    {
        if (!PlayerController.Instance.CanTakeDamage)
        {
            Debug.Log("Player is invincible. No damage taken.");
            return;
        }

        CurrentHealth -= amount;
        Debug.Log("Now Player Health: "+CurrentHealth);

        if (CurrentHealth <= 0)
        {
            PlayerController.Instance.Die();
            return;
        }

        isInvincible = true;
        StartCoroutine(ResetInvincible());
    }

    public void TakeHeal(float amount)
    {
        if (CurrentHealth + amount > MaxHealth)
            CurrentHealth = MaxHealth;
        else
            CurrentHealth += amount;

        Debug.Log("Now Player Health: " + CurrentHealth);
    }

    private IEnumerator ResetInvincible()
    {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
}
