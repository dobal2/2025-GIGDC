using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Gauge")]
    [SerializeField] private Image hpFillImage;
    [SerializeField] private Image energyFillImage;

    [Header("Weapon Display")]
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private Sprite spearSprite;
    [SerializeField] private Sprite bowSprite;
    [SerializeField] private Sprite bombSprite;

    private PlayerController player;
    private PlayerHealth health;
    private bool subscribed;

    public void OnEnable()
    {
        TryBind();
        RefreshAll();
    }

    public void OnDisable()
    {
        Unsubscribe();
    }

    public void Update()
    {
        if (!subscribed)
            TryBind();
    }

    private void TryBind()
    {
        if (player == null)
            player = PlayerController.Instance;
        if (health == null)
            health = PlayerHealth.Instance;
        if (player == null || health == null)
            return;

        player.OnEnergyChanged -= HandleEnergyChanged;
        player.OnChangeWeapon -= HandleWeaponChanged;
        health.OnHealthChanged -= HandleHealthChanged;

        player.OnEnergyChanged += HandleEnergyChanged;
        player.OnChangeWeapon += HandleWeaponChanged;
        health.OnHealthChanged += HandleHealthChanged;

        subscribed = true;
        RefreshAll();
    }

    private void Unsubscribe()
    {
        if (player != null)
        {
            player.OnEnergyChanged -= HandleEnergyChanged;
            player.OnChangeWeapon -= HandleWeaponChanged;
        }

        if (health != null)
            health.OnHealthChanged -= HandleHealthChanged;

        subscribed = false;
    }

    private void RefreshAll()
    {
        if (health != null)
            HandleHealthChanged(PlayerHealth.CurrentHealth, health.MaxHealth);

        if (player != null)
        {
            HandleEnergyChanged(player.CurrentEnergy, player.MaxEnergy);
            HandleWeaponChanged(player.AttackController != null ? player.AttackController.CurrentWeapon : PlayerController.CurrentWeapon);
        }
    }

    private void HandleHealthChanged(float current, float max)
    {
        if (hpFillImage != null)
            hpFillImage.fillAmount = max <= 0f ? 0f : Mathf.Clamp01(current / max);
    }

    private void HandleEnergyChanged(int current, int max)
    {
        if (energyFillImage != null)
            energyFillImage.fillAmount = max <= 0 ? 0f : Mathf.Clamp01(current / (float)max);
    }

    private void HandleWeaponChanged(WeaponType weapon)
    {
        if (weaponIconImage == null)
            return;

        weaponIconImage.sprite = weapon switch
        {
            WeaponType.Spear => spearSprite,
            WeaponType.Bow => bowSprite,
            WeaponType.Bomb => bombSprite,
            _ => null
        };

        weaponIconImage.enabled = weaponIconImage.sprite != null;
    }
}