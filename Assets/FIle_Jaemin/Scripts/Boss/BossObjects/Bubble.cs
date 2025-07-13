using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Bubble : Monster
{
    [SerializeField] protected Boss_Love boss;
    [SerializeField] protected float attackRadius;
    [SerializeField] protected GameObject bubblePopEffect;
    [SerializeField] protected float bossTakeDamage;
    private bool isSecondPhase;
    private AudioSource popSound;
    
    protected override void Awake()
    {
        
    }


    public void SetBoss(Boss_Love newBoss,bool isSecondPhase)
    {
        boss = newBoss;
        this.isSecondPhase = isSecondPhase;
    }

    protected override void Start()
    {
        popSound = GetComponent<AudioSource>();
        StartCoroutine(Explosion(5));
    }

    protected override void Attack()
    {
        
    }
    
    
    public IEnumerator Explosion(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        
        PlayBubblePopEffect();
        
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.CompareTag("Player"))
            {
                Debug.Log("Explosion");
                collidersEnemies[i].GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
        
        Destroy(gameObject);
    }

    protected void PlayBubblePopEffect()
    {
        // VFX 처리
        VisualEffect newPop = Instantiate(bubblePopEffect, transform.position, Quaternion.identity).GetComponent<VisualEffect>();
        newPop.Play();
        Destroy(newPop.gameObject, 2f);

        // SFX 처리 - 별도 오브젝트 생성해서 소리만 재생
        GameObject audioObj = new GameObject("TempAudio");
        audioObj.transform.position = transform.position;

        AudioSource tempAudio = audioObj.AddComponent<AudioSource>();
        tempAudio.clip = popSound.clip;
        tempAudio.volume = popSound.volume;
        tempAudio.pitch = popSound.pitch;
        tempAudio.spatialBlend = popSound.spatialBlend;
        tempAudio.Play();

        Destroy(audioObj, tempAudio.clip.length);
    }


    public override void TakeDamage(float amount)
    {
        hp -= amount;
        Die();
    }

    protected override void Die()
    {
        Debug.Log(isSecondPhase);
        if (isSecondPhase && boss != null)
        {
            boss.TakeDamage(bossTakeDamage);
        }
        PlayBubblePopEffect();
        Destroy(gameObject);
    }

    public override void KnockBack(Transform attacker, float knockBackForce, float knockBackAngle, float duration)
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
    
    
}
