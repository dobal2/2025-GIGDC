using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss_Excitement : Boss
{
    [Header("Prefabs")]
    [SerializeField] private GameObject homingMissilePrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject targetBoardPrefab;
    [SerializeField] private GameObject glassWallPrefab;
    [SerializeField] private GameObject glassBowlPrefab;
    [SerializeField] private GameObject clone;

    [Header("Transforms")]
    [SerializeField] private Transform missileSpawnPoint;
    [SerializeField] private Transform[] teleportPositions;
    [SerializeField] private Transform phase2Transform;
    [SerializeField] private Transform bombPos;
    [SerializeField] private Transform targetBoardPoint;

    [Header("Values")]
    [SerializeField] private float teleportCoolTime;
    private float teleportCoolTimer;
    private Vector3 lastTeleportedPos;

    [SerializeField] private float minBombThrowForce;
    [SerializeField] private float maxBombThrowForce;

    private bool isAttacking;

    protected override void Start()
    {
        clone.gameObject.SetActive(false);
        base.Start();
    }

    protected override void Attack()
    {
        
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
            StageManager.Objects--;
        }
    }

    public override void TakeDamage(float amount)
    {
        if (currentPhase == 1)
        {
            hp -= amount;
            TeleportRoutine();   
        }
    }
    
    public void TakeDamage(float amount,bool isTargetBoard)
    {
        hp -= amount;
    }

    private void Phase2()
    {
        anim.runtimeAnimatorController = phase2Anim;
        transform.position = phase2Transform.position + new Vector3(0, transform.localScale.y / 2, 0);
        currentPhase = 2;
        maxHp = phase2Hp;
        hp = maxHp;
        isAttacking = false;
        clone.gameObject.SetActive(true);

        StartCoroutine(SpawnTargetBoardLoop());
    }

    private IEnumerator PatternRoutine()
    {
        isAttacking = true;

        if (currentPhase == 1)
        {
            yield return StartCoroutine(FireHomingMissileRoutine());
            yield return StartCoroutine(ThrowBombAtPlayerRoutine());
        }
        else if (currentPhase == 2)
        {
            yield return StartCoroutine(GlassWallPattern());
            yield return StartCoroutine(GlassBowlPattern());
        }

        yield return new WaitForSeconds(4f);
        isAttacking = false;
    }

    private IEnumerator FireHomingMissileRoutine()
    {
        anim.SetTrigger("HomingMissile");
        GameObject missile = Instantiate(homingMissilePrefab, missileSpawnPoint.position, Quaternion.identity);
        missile.GetComponent<HomingMissile>().SetTarget(player);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator ThrowBombAtPlayerRoutine()
    {
        anim.SetTrigger("HomingMissile");
        
        int bombCount = Random.Range(4, 7);

        for (int i = 0; i < bombCount; i++)
        {
            GameObject bomb = Instantiate(bombPrefab, bombPos.position, Quaternion.identity);
            Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 toTarget = (player.position - bombPos.position).normalized;
                Vector3 upward = Vector3.up * Random.Range(0.3f, 1.2f);
                Vector3 throwDir = (toTarget + upward).normalized;
                float throwForce = Random.Range(minBombThrowForce, maxBombThrowForce);
                rb.linearVelocity = throwDir * throwForce;
            }
        }

        yield return new WaitForSeconds(1f);
    }
    

    private void TeleportRoutine()
    {
        teleportCoolTimer = 0;

        if (teleportPositions.Length > 1)
        {
            GameObject randomPlatform;
            do
            {
                randomPlatform = teleportPositions[Random.Range(0, teleportPositions.Length)].gameObject;
            } while (randomPlatform.transform.position == lastTeleportedPos);

            lastTeleportedPos = randomPlatform.transform.position;
            transform.position = randomPlatform.transform.position + new Vector3(0, randomPlatform.transform.localScale.y / 2, 0);
        }
        else if (teleportPositions.Length == 1)
        {
            Vector3 onlyPos = teleportPositions[0].position;
            lastTeleportedPos = onlyPos;
            transform.position = onlyPos + new Vector3(0, transform.localScale.y / 2, 0);
        }
    }

    private IEnumerator SpawnTargetBoardLoop()
    {
        while (currentPhase == 2)
        {
            GameObject board = Instantiate(targetBoardPrefab, targetBoardPoint.position, Quaternion.identity);
            board.GetComponent<TargetBoard>().SetBoss(this);
            yield return new WaitForSeconds(2f);
            if (board != null)
                Destroy(board);
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator GlassWallPattern()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(glassWallPrefab, targetBoardPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator GlassBowlPattern()
    {
        int bubbleCount = Random.Range(4, 6);
        float radius = 4f;
        Vector2 center = transform.position;

        List<GameObject> newGlassBowls = new List<GameObject>();

        for (int i = 0; i < bubbleCount; i++)
        {
            float angleDeg = -90f + (180f * i / (bubbleCount - 1));
            float angleRad = Mathf.Deg2Rad * angleDeg;

            Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;
            Vector2 spawnPos = center + offset;

            GameObject newGlassBowl = Instantiate(glassBowlPrefab, spawnPos, Quaternion.identity);
            newGlassBowls.Add(newGlassBowl);

            yield return new WaitForSeconds(0.1f);
        }

        foreach (var newGlassBowl in newGlassBowls)
        {
            if (newGlassBowl == null) continue;

            Vector2 direction = (player.transform.position - newGlassBowl.transform.position).normalized;
            float speed = 10f;

            Rigidbody2D rb = newGlassBowl.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * speed;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    protected override void Update()
    {
        if (hp <= 0) Die();
        
        if (!isAttacking)
        {
            StartCoroutine(PatternRoutine());
        }
        
        if (currentPhase == 1)
        {
            teleportCoolTimer += Time.deltaTime;
            if (teleportCoolTime <= teleportCoolTimer)
            {
                TeleportRoutine();
            }
        }
    }
}
