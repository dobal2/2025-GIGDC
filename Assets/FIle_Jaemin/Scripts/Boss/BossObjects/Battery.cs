using System;
using UnityEngine;
using UnityEngine.VFX;

public class Battery : Monster
{
    [SerializeField] private float health;
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform explosionTransform;
    [SerializeField] private float damage;
    [SerializeField] private GameObject explosionEffectPrefab;

    private AudioSource explosionSound;
    
    protected override void Awake()
    {
        explosionSound = GetComponent<AudioSource>();
    }
    

    public override void TakeDamage(float amount)
    {
        health -= damage;
    }

    protected override void Attack()
    {
        
    }

    protected override void Die()
    {
        // VFX
        GameObject newExplosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        newExplosionEffect.GetComponent<VisualEffect>().Play();
        Destroy(newExplosionEffect, 2f);

        // SFX - 분리된 오디오 재생용 오브젝트
        if (explosionSound != null && explosionSound.clip != null)
        {
            GameObject tempAudioObj = new GameObject("TempBatteryExplosionSound");
            tempAudioObj.transform.position = transform.position;

            AudioSource tempAudio = tempAudioObj.AddComponent<AudioSource>();
            tempAudio.clip = explosionSound.clip;
            tempAudio.volume = explosionSound.volume;
            tempAudio.pitch = explosionSound.pitch;
            tempAudio.spatialBlend = explosionSound.spatialBlend;
            tempAudio.outputAudioMixerGroup = explosionSound.outputAudioMixerGroup;
            tempAudio.Play();

            Destroy(tempAudioObj, tempAudio.clip.length);
        }
        else
        {
            Debug.LogWarning("Battery: explosionSound or clip not assigned.");
        }

        // 데미지 처리
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(explosionTransform.position, attackRadius);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].CompareTag("Boss"))
            {
                Boss_Frustration frustration = collidersEnemies[i].GetComponent<Boss_Frustration>();
                if (frustration)
                {
                    Debug.Log("Explosion");
                    frustration.TakeDamage(true, damage);
                }
            }
        }

        Destroy(gameObject);
    }


    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (explosionTransform != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(explosionTransform.position, attackRadius);   
        }
        
    }
}
