using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frustration : Boss
{
    private int takeDamageCount = 0;
    private bool canFlip = true;

    [Header("Phase1")]
    public GameObject windPrefab;
    public GameObject fingerPrefab;
    public Transform[] bossPositions;

    private int currentPosIndex = 0;

    [Header("Finger Attack Area")]
    [SerializeField] private float fingerMinX;
    [SerializeField] private float fingerMaxX;
    [SerializeField] private float fingerY;
    
    private float windTimer = 0f;
    private float fingerTimer = 0f;
    
    [Header("Phase2")]
    [SerializeField] private float dashForce;

    [SerializeField] private Transform fingerStretchPos;
    [SerializeField] private Vector2 fingerStretchAttackSize;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private GameObject lightObject;
    float direction = 1;

    private float dashTimer;
    private float lightTimer;
    
    [Header("Maps")] 
    [SerializeField] private GameObject phase1Map;
    [SerializeField] private GameObject phase2Map;

    protected override void Start()
    {
        lightObject.SetActive(false);
        base.Start();
        phase2Map.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        
        FlipToPlayerDirection();
        
        if (currentPhase == 1)
        {
            windTimer += Time.deltaTime;
            fingerTimer += Time.deltaTime;

            if (windTimer >= 7f)
            {
                StartCoroutine(CastWindAttack());
                windTimer = 0f;
            }

            if (fingerTimer >= 5f)
            {
                StartCoroutine(CastFingerAttack());
                fingerTimer = 0f;
            }

            if (takeDamageCount >= 5)
            {
                MoveBossRandomly();
                takeDamageCount = 0;
            }   
        }
        else if (currentPhase == 2)
        {
            dashTimer += Time.deltaTime;
            lightTimer += Time.deltaTime;
            
            if (dashTimer >= 5f)
            {
                if(!canFlip)
                    return;
                dashTimer = 0f;
                StartCoroutine(DashAttack());
            }
            
            if (lightTimer >= 7f)
            {
                lightTimer = 0f;
                StartCoroutine(LightAttack());
            }
            
            if (takeDamageCount >= 4)
            {
                takeDamageCount = 0;
                StretchFingerAttack();
            }   
        }
    }
    
    

    //Phase1 pattern
    IEnumerator CastWindAttack()
    {
        for (int i = 0; i < 2; i++)
        {
            if (player == null) yield break;

            Vector3 dir = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            GameObject wind = Instantiate(windPrefab, transform.position, Quaternion.Euler(0, 0, angle));
            Rigidbody2D rb = wind.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * 5f;
            }

            yield return new WaitForSeconds(0.6f);
        }
    }

    IEnumerator CastFingerAttack()
    {
        int count = Random.Range(5, 9);
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(fingerMinX, fingerMaxX),
                fingerY,
                0f
            );

            Instantiate(fingerPrefab, randomPos, Quaternion.identity);
            yield return new WaitForSeconds(0.4f);
        }
    }

    void MoveBossRandomly()
    {
        if (bossPositions.Length < 2) return;

        currentPosIndex = (currentPosIndex + 1) % bossPositions.Length;
        transform.position = bossPositions[currentPosIndex].position;
    }
    
    //Phase2 Pattern
    IEnumerator DashAttack()
    {
        Debug.Log("Dash");
        canFlip = false;
        if (player.transform.position.x < transform.position.x)
        {
            direction = -1;
        }
        else if (player.transform.position.x > transform.position.x)
        {
            direction = 1;
        }
        
        rigid.AddForce(new Vector2(dashForce * direction,0),ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.7f);
        
        rigid.linearVelocity = Vector2.zero;

        canFlip = true;
    }

    private void StretchFingerAttack()
    {
        Collider2D[] collidersEnemies = Physics2D.OverlapBoxAll(fingerStretchPos.position, fingerStretchAttackSize, 0);
        foreach (var collider in collidersEnemies)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    private void SummonBattery()
    {
        int count = Random.Range(2, 4); // 2 또는 3

        HashSet<int> usedOffsets = new HashSet<int>(); // 중복 위치 방지

        for (int i = 0; i < count; i++)
        {
            int randomXOffset;

            // 중복되지 않는 X 위치를 고를 때까지 반복
            do
            {
                randomXOffset = Random.Range(-4, 5); // -7 ~ 7
            }
            while (usedOffsets.Contains(randomXOffset) || (randomXOffset > -1 && randomXOffset < 1));


            usedOffsets.Add(randomXOffset);

            Vector2 spawnPosition = new Vector2(transform.position.x + randomXOffset, transform.position.y);
            GameObject newBattery = Instantiate(batteryPrefab, spawnPosition, Quaternion.identity);

            // 필요시 이후에 관리하도록 리스트에 추가하거나 태그 설정 가능
            // spawnedBatteries.Add(newBattery);
        }
    }


    IEnumerator LightAttack()
    {
        canFlip = false;
        
        SummonBattery();
        lightObject.transform.rotation = Quaternion.Euler(0, 0, 125f);
        lightObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        
        anim.SetTrigger("LightAttack");

    }

    public override void TakeDamage(float amount)
    {
        takeDamageCount++;
        if (currentPhase == 1)
        {
            base.TakeDamage(amount);   
        }
        else if (currentPhase == 2)
        {
            base.TakeDamage(1);
        }
    }
    
    public void TakeDamage(bool batteryExplosion,float explosionDamage)
    {
        base.TakeDamage(explosionDamage);
        StopAllCoroutines();
        lightObject.SetActive(false);
        Debug.Log("ExplosionDamaged");
    }
    
    private void Phase2()
    {
        takeDamageCount = 0;
        phase1Map.SetActive(false);
        phase2Map.SetActive(true);
        currentPhase = 2;
        maxHp = phase2Hp;
        hp = maxHp;
        //anim.runtimeAnimatorController = phase2Anim;

        //transform.position = new Vector2(0, 7);
    }

    private void FlipToPlayerDirection()
    {
        if(!canFlip)
            return;
        if (player.transform.position.x < transform.position.x && !facingRight)
        {
           Flip(); 
        }
        else if (player.transform.position.x > transform.position.x && facingRight)
        {
            Flip();
        }
    }

    public void LightAnimEnd()
    {
        lightObject.SetActive(false);
        canFlip = true;
    }

    protected override void Die()
    {
        StopAllCoroutines();
        if (currentPhase == 1)
        {
            Phase2();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    private void OnDrawGizmosSelected()
        {
            if (fingerStretchPos != null)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                Gizmos.DrawCube(fingerStretchPos.position, fingerStretchAttackSize);
            }
        }
}
