using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class HeartBubble : Bubble
{
    [SerializeField] private GameObject shieldPrefab;

    [SerializeField] private float invincibleTime;
    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(Explosion(5));
    }
    
    protected override void Die()
    {
        if (player != null)
        {
            GameObject shield = Instantiate(shieldPrefab, player.position + new Vector3(0,player.localScale.y/2,0), Quaternion.identity);
            player.GetComponentInChildren<PlayerHealth>().SetInvincibleFor(invincibleTime);
        }
        else
        {
            Debug.LogWarning("Player not found");
        }
        PlayBubblePopEffect();
        Destroy(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
    
    
}