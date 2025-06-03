using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    public float maxHealth = 5;
    [SerializeField] private float invincibleTime = 1f;

    private float Currnthealth;
    [HideInInspector] public bool isInvincible;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Currnthealth = maxHealth;
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

        Currnthealth -= damage;
        Debug.Log("Now Player Health: "+Currnthealth);

        if (Currnthealth <= 0)
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
