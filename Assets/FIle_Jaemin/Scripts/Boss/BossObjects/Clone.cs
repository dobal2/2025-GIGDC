using System.Collections;
using UnityEngine;

public class Clone : Monster
{
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDistance = 1.8f;
    [SerializeField] private Transform attackCheck;
    private AudioSource attackSound;
    private bool isBorn;

    private float lastAttackTime;
    private bool isAttacking = false;
    
    protected override void Awake()
    {
        
    }

    protected override void Start()
    {
        attackSound = GetComponent<AudioSource>();
        base.Start();
        anim.SetTrigger("Born");
        StartCoroutine(WaitToBorn());
    }

    IEnumerator WaitToBorn()
    {
        yield return new WaitForSeconds(1.5f);
        isBorn = true;

    }

    protected void Update()
    {
        if(!isBorn)
            return;
        
        if (player == null)
        {
            Debug.LogWarning("플레이어 없음");
            return;
        }

        if (!isAttacking)
        {
            float dirX = player.position.x - transform.position.x;
            if (dirX != 0)
                transform.localScale = new Vector3(Mathf.Sign(dirX), 1f, 1f);
        }
        
        if (isAttacking || !canAttack) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            anim.SetBool("isWalking", true);
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(dir * speed * Time.deltaTime);

            if (dir.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(dir.x), 1f, 1f);
        }
        else
        {
            anim.SetBool("isWalking", false);

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
        attackSound.Play();
    }
    
    

    public void AttackOverlapCircle()
    {
        if (attackCheck == null)
        {
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackRange);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                target.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        StartCoroutine(WaitToAttack(attackCoolDown));
    }

    protected override void Die()
    {
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
