using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LowMonster_Rare_inferior : Monster
{
    [SerializeField] private GameObject inferiorAttackLineStartPrefab;
    [SerializeField] private GameObject inferiorAttackLineEndPrefab;
    [SerializeField] private float lineSpacing = 0.5f;
    [SerializeField] private int lineCount = 5;
    [SerializeField] private float lineDuration = 0.5f;

    private bool didVerticalAttack;
    

    protected override void Start()
    {
        base.Start();
        if(inferiorAttackLineStartPrefab == null || inferiorAttackLineEndPrefab == null)
            Debug.LogError("No inferiorAttackLineStartPrefab or inferiorAttackLineEndPrefab assigned");
            
    }

    protected override void Attack()
    {
        anim.SetTrigger("Attack");

        if (didVerticalAttack)
        {
            didVerticalAttack = false;
            StartCoroutine(DoHorizontalAttack());
        }
        else
        {
            didVerticalAttack = true;
            StartCoroutine(DoVerticalAttack());
        }
    }

    public override void KnockBack(Transform attacker, float knockBackForce, float knockBackAngle, float duration)
    {
        
    }


    protected override void Die()
    {
        base.Die();
    }

    protected void Update()
    {
        if (canAttack && !isCountering && !isCounterStunned)
        {
            // 카운터 시도, 실패하면 공격
            if (!TryCounter())
            {
                Attack();
                StartCoroutine(WaitToAttack(attackCoolDown));
            }
        }
    }

    private IEnumerator DoVerticalAttack()
    {
        yield return SpawnSymmetricLines(Vector3.up);
    }

    private IEnumerator DoHorizontalAttack()
    {
        yield return SpawnSymmetricLines(Vector3.right);
    }

    
    private IEnumerator SpawnSymmetricLines(Vector3 direction)
    {
        int half = lineCount / 2;
        Vector3 centerPos = transform.position;

        for (int i = 0; i <= half; i++)
        {
            float offset = i * lineSpacing;

            // 1. +방향
            Vector3 pos1 = centerPos + direction * offset;
            Quaternion rot1 = GetRotationFromDirection(direction);
            GameObject obj1 = InstantiateLinePrefab(i, pos1, rot1);
            Destroy(obj1, lineDuration);

            // 2. -방향 (중앙 제외)
            if (i > 0)
            {
                Vector3 pos2 = centerPos - direction * offset;
                Quaternion rot2 = GetRotationFromDirection(-direction);
                GameObject obj2 = InstantiateLinePrefab(i, pos2, rot2);
                Destroy(obj2, lineDuration);
            }

            yield return new WaitForSeconds(0.03f);
        }
    }


    private GameObject InstantiateLinePrefab(int index, Vector3 position, Quaternion rotation)
    {
        GameObject prefab;

        if (index == 0)
            prefab = inferiorAttackLineStartPrefab;
        else if (index == lineCount / 2)
            prefab = inferiorAttackLineEndPrefab;
        else
            prefab = inferiorAttackLineStartPrefab; // 중간도 동일 프리팹

        return Instantiate(prefab, position, rotation);
    }
    
    private Quaternion GetRotationFromDirection(Vector3 dir)
    {
        if (dir == Vector3.right)
            return Quaternion.Euler(0, 0, -90);
        else if (dir == Vector3.left)
            return Quaternion.Euler(0, 0, 90);
        else if (dir == Vector3.up)
            return Quaternion.Euler(0, 0, 0);
        else if (dir == Vector3.down)
            return Quaternion.Euler(0, 0, 180); // 또는 270
        else
            return Quaternion.identity;
    }


    public override void TakeDamage(float amount)
    {
        GameObject newInkExplosion;
        
        // 카운터 중이라면 카운터 중단하고 즉시 기절 상태로 전환
        if (isCountering)
        {
            if (counterCoroutine != null)
            {
                StopCoroutine(counterCoroutine);
                counterCoroutine = null;
            }
            
            isCountering = false;
            
            // 데미지 적용
            hp -= amount;
            newInkExplosion = Instantiate(inkHitEffect, transform.position, Quaternion.identity);
            Destroy(newInkExplosion,2);
            
            if (hp <= 0)
            {
                Die();
                return;
            }
            
            // 기절 코루틴 시작하고 바로 종료 (일반 피격 처리 건너뜀)
            if (counterStunCoroutine != null)
            {
                StopCoroutine(counterStunCoroutine);
            }
            counterStunCoroutine = StartCoroutine(CounterStunRoutine());
            return;  // 카운터 피격은 여기서 종료
        }
        
        // 카운터 기절 중이면 대미지 50% 증가 (1.5배)
        if (isCounterStunned)
        {
            amount *= 1.5f;
        }
        
        hp -= amount;
        newInkExplosion = Instantiate(inkHitEffect, transform.position, Quaternion.identity);
        Destroy(newInkExplosion,2);
        
        if (hp <= 0) Die();
        Debug.Log("inferior 넉백,경직 무시됨");
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
