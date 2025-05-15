using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossExcitement : Monster
{
    [SerializeField] private GameObject homingMissilePrefab;
    [SerializeField] private Transform missileSpawnPoint;
    [SerializeField] private Transform[] teleportPositions; // 랜덤 텔레포트 위치들
    [SerializeField] private float moveCoolTime;
    private float moveCoolTimer;
    private Vector3 lastTeleportedPos;

    private bool isAttacking;
    private bool canTeleport = true;

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

            // 4초 동안 한 자리에 있음
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

        if (canTeleport)
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    private IEnumerator TeleportRoutine()
    {
        moveCoolTimer = 0;
        canTeleport = false;

        // 랜덤 위치 선택
        if (teleportPositions.Length > 0)
        {
            Vector3 randomPos = teleportPositions[Random.Range(0, teleportPositions.Length)].position;
            randomPos += transform.localScale / 2;
            transform.position = randomPos;
        }

        // 텔레포트 후 쿨타임 (예: 1초)
        yield return new WaitForSeconds(1f);
        canTeleport = true;
    }

    private void Update()
    {
        moveCoolTimer += Time.deltaTime;
        if (moveCoolTime <= moveCoolTimer)
        {
            StartCoroutine(TeleportRoutine());
        }
    }
}