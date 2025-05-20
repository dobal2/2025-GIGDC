using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossExcitement : Monster
{
    [Header("Prefabs")]
    [SerializeField] private GameObject homingMissilePrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject thornPrefab;
    
    [SerializeField] private Transform missileSpawnPoint;
    [SerializeField] private Transform[] teleportPositions; // 랜덤 텔레포트 위치들
    [SerializeField] private Transform bombPos;
    
    
    [SerializeField] private float teleportCoolTime;
    private float teleportCoolTimer;
    private Vector3 lastTeleportedPos;
    
    [SerializeField] private float minBombThrowForce;
    [SerializeField] private float maxBombThrowForce;

    private bool isAttacking;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(PatternRoutine());
    }

    protected override void Attack() { }
    protected override void Die() { }

    private IEnumerator PatternRoutine()
    {
        while (true)
        {
            isAttacking = true;
            FireHomingMissile();
            StartCoroutine(ThornPattern());

            // 4초 동안 한 자리에 있음
            yield return new WaitForSeconds(4f);
            
            ThrowBombAtPlayerPattern();
            
            

            isAttacking = false;
        }
    }

    private void FireHomingMissile()
    {
        GameObject missile = Instantiate(homingMissilePrefab, missileSpawnPoint.position, Quaternion.identity);
        missile.GetComponent<HomingMissile>().SetTarget(player);
    }

    public void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        TeleportRoutine();
    }

    private void TeleportRoutine()
    {
        teleportCoolTimer = 0;

        if (teleportPositions.Length > 1)
        {
            Vector3 randomPos;
            do
            {
                randomPos = teleportPositions[Random.Range(0, teleportPositions.Length)].position;
            } while (randomPos == lastTeleportedPos);

            lastTeleportedPos = randomPos;
            randomPos += new Vector3(0, transform.localScale.y / 2, 0);
            transform.position = randomPos;
        }
        else if (teleportPositions.Length == 1)
        {
            Vector3 onlyPos = teleportPositions[0].position;
            lastTeleportedPos = onlyPos;
            transform.position = onlyPos + new Vector3(0, transform.localScale.y / 2, 0);
        }
        
    }

    public void ThrowBombAtPlayerPattern()
    {
        if (player == null) return;

        int bombCount = Random.Range(4, 7);

        for (int i = 0; i < bombCount; i++)
        {
            
            GameObject bomb = Instantiate(bombPrefab,bombPos.position,Quaternion.identity);
            Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 toTarget = (player.position - bombPos.position).normalized;
                Vector3 upward = Vector3.up * Random.Range(0f,1f);
                Vector3 throwDir = (toTarget + upward).normalized;

                float throwForce = Random.Range(minBombThrowForce, maxBombThrowForce);
                
                rb.linearVelocity = throwDir * throwForce;
            }
            else
            {
                Debug.Log("bomb rigid null");
            }
        }
    }
    
    IEnumerator ThornPattern()
    {
        int thornCount = 12;
        float radius = 4f; // 버블이 퍼질 반지름
        Vector2 center = transform.position; // 또는 원하는 중심 좌표

        for (int i = 0; i < thornCount; i++)
        {
            float angle = i * Mathf.PI * 2f / thornCount; // 각도를 나눠서 원형 배치
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector2 spawnPos = center + offset;

            GameObject newBubble = Instantiate(thornPrefab, spawnPos, Quaternion.identity);
            newBubble.GetComponent<Bubble>().damage = damage;
        }

        yield return new WaitForSeconds(1f);
    }

    
    
    

    private void Update()
    {
        teleportCoolTimer += Time.deltaTime;
        if (teleportCoolTime <= teleportCoolTimer)
        {
            TeleportRoutine();
        }
    }
}