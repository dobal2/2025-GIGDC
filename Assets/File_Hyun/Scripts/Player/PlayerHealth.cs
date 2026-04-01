using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    public event Action<float, float> OnHealthChanged;

    [HideInInspector] public float MaxHealth = 5;
    public static float CurrentHealth = 5;
    [SerializeField] private float invincibleTime = 1f;
    [SerializeField] private bool DeveloperMode = false;

    [HideInInspector] public bool isInvincible;

    private Coroutine resetInvincibleCoroutine;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        NotifyHealthChanged();
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0)
            return;

        if (!PlayerController.Instance.CanTakeDamage)
        {
            Debug.Log("Player is invincible. No damage taken.");
            return;
        }

        if (!DeveloperMode)
            CurrentHealth -= amount;

        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        NotifyHealthChanged();
        Debug.Log("Now Player Health: " + CurrentHealth);

        if (CurrentHealth <= 0)
        {
            PlayerController.Instance.Die();
            return;
        }

        PlayerController.Instance.PlayClip(PlayerController.Instance.Damage);
        isInvincible = true;
        resetInvincibleCoroutine = StartCoroutine(ResetInvincible());
    }

    public void TakeHeal(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, MaxHealth);
        NotifyHealthChanged();
        Debug.Log("Now Player Health: " + CurrentHealth);
    }

    public void SetInvincibleFor(float duration)
    {
        if (resetInvincibleCoroutine != null)
        {
            StopCoroutine(resetInvincibleCoroutine);
            resetInvincibleCoroutine = null;
        }

        StartCoroutine(SetInvincibleCoroutine(duration));
    }

    private IEnumerator SetInvincibleCoroutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    private IEnumerator ResetInvincible()
    {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}