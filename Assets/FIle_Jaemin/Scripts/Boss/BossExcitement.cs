using System;
using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossExcitement : Monster
{
    [Header("Prefabs")]
    [SerializeField] private GameObject homingMissilePrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject targetBoardPrefab;
    
    [Header("Transforms")]
    [SerializeField] private Transform missileSpawnPoint;
    [SerializeField] private Transform[] teleportPositions; // 랜덤 텔레포트 위치들
    [SerializeField] private Transform bombPos;
    [SerializeField] private Transform targetBoardPoint;
    
    [Header("Values")]
    [SerializeField] private int phase = 1;
    [SerializeField] private float phase2Hp;
    
    
    [SerializeField] private float teleportCoolTime;
    private float teleportCoolTimer;
    private Vector3 lastTeleportedPos;
    
    [SerializeField] private float minBombThrowForce;
    [SerializeField] private float maxBombThrowForce;

    private bool isAttacking;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(PatternRoutine());
        }
    }

    protected override void Die()
    {
        StopAllCoroutines();
        if (phase == 1)
        {
            Phase2();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Phase2()
    {
        phase = 2;
        maxHp = phase2Hp;
        hp = maxHp;
        isAttacking = false;
    }

    private IEnumerator PatternRoutine()
    {
        while (true)
        {
            isAttacking = true;

            if (phase == 1)
            {
                FireHomingMissile();
            
                ThrowBombAtPlayerPattern();
            }
            else if (phase == 2)
            {
                yield return StartCoroutine(SpawnTargetBoard());
            }
            
            yield return new WaitForSeconds(4f);
            
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
    
    
    //phase2 pattern
    IEnumerator SpawnTargetBoard()
    {
        yield return new WaitForSeconds(3);

        GameObject newTargetBoard = Instantiate(targetBoardPrefab,targetBoardPoint.position,Quaternion.identity);
        
        yield return new WaitForSeconds(2);
        
        Destroy(newTargetBoard);
        
    }
    
    private void Update()
    {
        if (hp <= 0) Die();
        
        Attack();

        if (phase == 1)
        {
            teleportCoolTimer += Time.deltaTime;
            if (teleportCoolTime <= teleportCoolTimer)
            {
                TeleportRoutine();
            }   
        }
    }
}