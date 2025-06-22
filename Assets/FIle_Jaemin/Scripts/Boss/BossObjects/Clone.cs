using UnityEngine;

public class Clone : Monster
{
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDistance = 1.8f;
    [SerializeField] private Transform attackCheck;

    private float lastAttackTime;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        Debug.Log("🟢 Clone Start - Initialized");
    }

    protected override void Update()
    {
        base.Update();

        if (player == null)
        {
            Debug.LogWarning("❗ 플레이어 없음");
            return;
        }

        Debug.Log($"🟡 Update - canAttack: {canAttack}, isAttacking: {isAttacking}");

        if (isAttacking || !canAttack) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Debug.Log($"📏 거리: {distance:F2}, 공격거리: {attackDistance}");

        if (distance > attackDistance)
        {
            anim.SetBool("isWalking", true);
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(dir * speed * Time.deltaTime);

            if (dir.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(dir.x), 1f, 1f);

            Debug.Log("🚶 이동 중");
        }
        else
        {
            anim.SetBool("isWalking", false);
            Debug.Log("⛔ 멈춤 - 공격 범위 도달");

            if (Time.time - lastAttackTime >= attackCoolDown)
            {
                lastAttackTime = Time.time;
                Attack();
            }
            else
            {
                Debug.Log($"⌛ 공격 쿨다운 중: {Time.time - lastAttackTime:F2}s / {attackCoolDown}s");
            }
        }
    }

    protected override void Attack()
    {
        isAttacking = true;
        canAttack = false;
        anim.SetTrigger("Attack");
        Debug.Log("🔴 Attack Trigger 실행됨");
    }

    public void AttackOverlapCircle()
    {
        if (attackCheck == null)
        {
            Debug.LogWarning("❗ attackCheck가 설정되지 않음");
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackRange);
        Debug.Log($"🎯 AttackOverlapCircle - 타겟 수: {colliders.Length}");

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                Debug.Log("✅ 플레이어 적중! 데미지 적용");
                target.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    public void EndAttack()
    {
        Debug.Log("🟩 EndAttack - 공격 종료");
        isAttacking = false;
        StartCoroutine(WaitToAttack(attackCoolDown));
    }

    protected override void Die()
    {
        Debug.Log("💀 Clone 사망");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackCheck != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(attackCheck.position, attackRange);
        }
    }
}
